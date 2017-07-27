using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Reflection.Emit;
using System.Diagnostics;
using Pixel3D.Serialization.Discovery.ReadIL;

namespace Pixel3D.Serialization.Discovery
{
    internal static class DelegateDiscovery
    {
        public static List<DelegateUsage> Run(IEnumerable<Assembly> assemblies, StreamWriter report, StreamWriter errors)
        {
            Debug.Assert((report != null) == (errors != null));

            List<DelegateUsage> allDelegateUsage = new List<DelegateUsage>(400);

            const BindingFlags bindingFlagsForAllMembers = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            var allTypes = assemblies.SelectMany(a => a.GetTypes());

            foreach(var type in allTypes)
            {
                var methods = type.GetConstructors(bindingFlagsForAllMembers).Cast<MethodBase>().Concat(type.GetMethods(bindingFlagsForAllMembers).Cast<MethodBase>());

                foreach(var method in methods)
                {
                    if(method.GetCustomAttributes(typeof(SerializationIgnoreDelegatesAttribute), false).Length > 0)
                        continue;


                    bool reportHeaderWritten = false;

                    foreach(var d in GetAllDelegateCreationIn(method, type))
                    {
                        bool methodAndTargetKnown;
                        if(methodAndTargetKnown = (d.delegateMethod != null && d.targetTypeKnown))
                        {
                            allDelegateUsage.Add(new DelegateUsage { targetType = d.targetType, delegateMethod = d.delegateMethod, delegateType = d.delegateType });
                        }

                        if(report != null && errors != null)
                        {
                            if(!reportHeaderWritten)
                            {
                                report.WriteLine(method.DeclaringType.ToString());
                                report.WriteLine("  " + method.ToString());
                                reportHeaderWritten = true;
                            }
                            d.WriteInfo(report);
                            report.WriteLine();

                            // Errors:
                            if(!methodAndTargetKnown)
                            {
                                errors.WriteLine("ERROR: Delegate discovery failed to determine delegate info from IL");
                                d.WriteInfo(errors, true);
                                errors.WriteLine();
                            }

                            if((d.delegateMethod != null && d.delegateMethod.ContainsGenericParameters) || d.delegateType.ContainsGenericParameters)
                            {
                                errors.WriteLine("WARNING: Delegate with open-constructed type and/or method"); // Pretty sure it will always be both
                                d.WriteInfo(errors, true);
                                errors.WriteLine();
                            }
                        }
                    }
                }
            }

            return allDelegateUsage;
        }



        private static IEnumerable<DelegateUsageInternal> GetAllDelegateCreationIn(MethodBase method, Type methodDeclaringType)
        {
            Module module = method.Module;

            var methodBody = method.GetMethodBody();
            if(methodBody == null) // Can happen with abstract methods, externs (P/Invoke or delegate constructors), possibly other cases
                yield break;

            byte[] ilBytes = methodBody.GetILAsByteArray();
            MemoryStream ilStream = new MemoryStream(ilBytes);
            BinaryReader br = new BinaryReader(ilStream);

            // Track what is on the stack as we go into the delegate constructor
            List<DelegateUsageInternal> stack = new List<DelegateUsageInternal>();

            long ilOffset;
            while((ilOffset = ilStream.Position) < ilStream.Length)
            {
                OpCode opCode = br.ReadOpCode();

                // Find delegates by searching for IL instructions in this order:
                // <Target Reference>   = ldnull | ldarg | ldloc | ldfld | dup
                // <Method Pointer>     = ldftn | ldvirtftn
                // <Construct Delegate> = newobj

                ushort opCodeValue = (ushort)opCode.Value;
                switch(opCodeValue)
                {
                    case OpCodeValues.Dup:
                        if(stack.Count > 0)
                            stack.Add(stack[stack.Count-1]); // Duplicate
                        break;

                    //
                    // Load the target (1st operation)
                    //
                    case OpCodeValues.Ldnull:
                        stack.Add(new DelegateUsageInternal() { targetTypeKnown = true, targetType = null });
                        break;

                    case OpCodeValues.Ldarg_0:
                    case OpCodeValues.Ldarg_1:
                    case OpCodeValues.Ldarg_2:
                    case OpCodeValues.Ldarg_3:
                    case OpCodeValues.Ldarg_S:
                    case OpCodeValues.Ldarg:
                        {
                            int index = br.ReadIndexOperandLdarg(opCodeValue);
                            if(!method.IsStatic) // For an instance method, need to include the "this" pointer in the arguments
                                index--;
                            var parameterType = (index >= 0 ? method.GetParameters()[index].ParameterType : method.DeclaringType);
                            stack.Add(new DelegateUsageInternal() { targetTypeKnown = true, targetType = parameterType });
                        }
                        break;

                    case OpCodeValues.Ldloc_0:
                    case OpCodeValues.Ldloc_1:
                    case OpCodeValues.Ldloc_2:
                    case OpCodeValues.Ldloc_3:
                    case OpCodeValues.Ldloc_S:
                    case OpCodeValues.Ldloc:
                        {
                            int index = br.ReadIndexOperandLdloc(opCodeValue);
                            var localType = method.GetMethodBody().LocalVariables.Where(lvi => lvi.LocalIndex == index).Select(lvi => lvi.LocalType).FirstOrDefault();
                            stack.Add(new DelegateUsageInternal() { targetTypeKnown = (localType != null), targetType = localType });
                        }
                        break;

                    case OpCodeValues.Ldfld:
                        {
                            var fieldType = method.ResolveFieldFromMethod(br.ReadInt32(), methodDeclaringType).FieldType;
                            stack.Add(new DelegateUsageInternal() { targetTypeKnown = (fieldType != null), targetType = fieldType });
                        }
                        break;

                    //
                    // Load the method (2nd operation)
                    //
                    case OpCodeValues.Ldftn:
                    case OpCodeValues.Ldvirtftn:
                        stack.Add(new DelegateUsageInternal() { delegateMethod = method.ResolveMethodFromMethod(br.ReadInt32(), methodDeclaringType) as MethodInfo });
                        break;

                    //
                    // Create the delegate (3rd operation)
                    //
                    case OpCodeValues.Newobj:
                        {
                            ConstructorInfo potentialDelegateConstructor = method.ResolveMethodFromMethod(br.ReadInt32(), methodDeclaringType) as ConstructorInfo;
                            if(potentialDelegateConstructor != null)
                            {
                                Type potentialDelegateType = potentialDelegateConstructor.DeclaringType;
                                if(potentialDelegateType.IsSubclassOf(typeof(MulticastDelegate)) && CheckDelegateConstructorParameters(potentialDelegateConstructor))
                                {
                                    if(potentialDelegateType.GetCustomAttributes(typeof(SerializableDelegateAttribute), true).Length > 0)
                                    {
                                        // Reconstruct what's going into the delegate constructor by looking at what's on the stack
                                        yield return new DelegateUsageInternal
                                        {
                                            instantiatingMethod = method,
                                            instantiationILOffset = ilOffset,
                                            targetTypeKnown = (stack.Count >= 2 ? stack[stack.Count-2].targetTypeKnown : false),
                                            targetType      = (stack.Count >= 2 ? stack[stack.Count-2].targetType      : null),
                                            delegateMethod  = (stack.Count >= 1 ? stack[stack.Count-1].delegateMethod  : null),
                                            delegateType = potentialDelegateType
                                        };
                                    }
                                }
                            }

                            stack.Clear();
                        }
                        break;

                    //
                    // Not a delegate:
                    //
                    default:
                        br.SkipOperand(opCode.OperandType);
                        stack.Clear(); // Instruction could be anything, invalidating the stack we've built up
                        break;
                }
            }
        }


        static bool CheckDelegateConstructorParameters(ConstructorInfo constructorInfo)
        {
            if(constructorInfo != null)
            {
                var parameters = constructorInfo.GetParameters();
                if(parameters.Length == 2)
                {
                    if(parameters[0].ParameterType == typeof(object)
                            && parameters[0].Attributes == ParameterAttributes.None
                            && parameters[1].ParameterType == typeof(IntPtr)
                            && parameters[1].Attributes == ParameterAttributes.None)
                    {
                        return true;
                    }
                }
            }

            return false;
        }



        public static void WriteDelegateUsageGrouped(List<DelegateUsage> delegates, StreamWriter report)
        {
            foreach(var typeGroup in delegates.GroupBy(d => d.delegateType))
            {
                report.WriteLine(typeGroup.Key);
                foreach(var methodGroup in typeGroup.GroupBy(d => d.delegateMethod))
                {
                    report.WriteLine("  " + methodGroup.Key);
                    foreach(var target in methodGroup.Select(d => d.targetType).Distinct())
                    {
                        report.WriteLine("    " + (target != null ? target.ToString() : "(null)"));
                    }
                }
            }
        }

    }
}

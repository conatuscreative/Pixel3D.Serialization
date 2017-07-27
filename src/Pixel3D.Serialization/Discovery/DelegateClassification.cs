using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Pixel3D.Serialization.BuiltIn.DelegateHandling;

namespace Pixel3D.Serialization.Discovery
{
    class DelegateClassification
    {
        public DelegateClassification(HashSet<Type> delegateFieldTypes, List<DelegateUsage> delegates)
        {
            this.delegateFieldTypes = delegateFieldTypes;

            delegateLookup = delegates.Where(d => !d.delegateType.ContainsGenericParameters).ToLookup(d => d.delegateType);

            // For some reason the "generics" we get back are not the actual generic defintions, so need converting:
            openConstructedDelegateLookup = delegates.Where(d => d.delegateType.ContainsGenericParameters)
                    .Select(d => new DelegateUsage
                    {
                        delegateType = d.delegateType.GetGenericTypeDefinition(),
                        delegateMethod = d.delegateMethod.GetGenericMethodDefinition(),
                        targetType = d.targetType
                    })
                    .ToLookup(d => d.delegateType);
        }


        HashSet<Type> delegateFieldTypes;
        ILookup<Type, DelegateUsage> delegateLookup;
        ILookup<Type, DelegateUsage> openConstructedDelegateLookup;

        public IEnumerable<DelegateUsage> GetDelegateUsageForDelegateType(Type type)
        {
            Debug.Assert(!type.ContainsGenericParameters); // <- can't handle unspecified generic field type

            IEnumerable<DelegateUsage> output = null;

            if(delegateLookup.Contains(type))
                output = delegateLookup[type];

            if(type.IsGenericType)
            {
                // See if we can find an open generic usage that can be mapped onto this type in its generic form
                // (This is to support RCRU "NoBehaviour"-and-similar generic delegate creation, and will probably do weird things if given partially-closed generics)
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if(openConstructedDelegateLookup.Contains(genericTypeDefinition))
                {
                    var genericUsages = openConstructedDelegateLookup[genericTypeDefinition];
                    // Assume that the generic instantiation will be run in a concrete form, with the right generic arguments to be assigned to the given field:
                    var concreteUsages = genericUsages.Select(d => new DelegateUsage
                            {
                                delegateType = type,
                                delegateMethod = d.delegateMethod.MakeGenericMethod(type.GetGenericArguments()),
                                targetType = d.targetType
                            });

                    if(output != null)
                        output = output.Concat(concreteUsages);
                    else
                        output = concreteUsages;
                }

            }

            return output ?? Enumerable.Empty<DelegateUsage>();
        }



        public Dictionary<Type, DelegateTypeInfo> GenerateDelegateTypeTable()
        {
            Dictionary<Type, DelegateTypeInfo> delegateTypeTable = new Dictionary<Type, DelegateTypeInfo>();

            foreach(var delegateFieldType in delegateFieldTypes)
            {
                var methodsForDelegateType = GetDelegateUsageForDelegateType(delegateFieldType).GroupBy(d => d.delegateMethod);

                List<DelegateMethodInfo> methodInfoList = methodsForDelegateType.Select(methodGroup =>
                {
                    bool canHaveTarget = methodGroup.Any(d => d.targetType != null);
                    return new DelegateMethodInfo(methodGroup.Key, canHaveTarget);
                })
                .NetworkOrder(dmi => (dmi.method.DeclaringType.ToString() + " " + dmi.method.ToString())).ToList();

                delegateTypeTable.Add(delegateFieldType, new DelegateTypeInfo(methodInfoList));
            }

            return delegateTypeTable;
        }
        


        public void Report(StreamWriter report, StreamWriter errors)
        {
            report.WriteLine("-------------------");
            report.WriteLine("Open Delegate Types");
            report.WriteLine("-------------------");
            report.WriteLine();

            foreach(var item in openConstructedDelegateLookup)
            {
                report.WriteLine(item.Key);
                foreach(var usage in item)
                {
                    report.WriteLine("  " + usage.delegateMethod);
                }
            }

            report.WriteLine();
            report.WriteLine();
            report.WriteLine();
            report.WriteLine();
            report.WriteLine("-----------------");
            report.WriteLine("Delegate Dispatch");
            report.WriteLine("-----------------");
            report.WriteLine();

            int maxUsageCountByType = 0;
                
            foreach(var delegateFieldType in delegateFieldTypes)
            {
                var methodsForDelegateType = GetDelegateUsageForDelegateType(delegateFieldType).GroupBy(d => d.delegateMethod);

                int usageCountForType = methodsForDelegateType.Count();
                if(usageCountForType > maxUsageCountByType)
                    maxUsageCountByType = usageCountForType;

                report.WriteLine(delegateFieldType + " (" + usageCountForType + ")");

                foreach(var methodGroup in methodsForDelegateType)
                {
                    var targetTypes = methodGroup.Select(d => d.targetType).Distinct();

                    report.WriteLine("  " + methodGroup.Key.DeclaringType + "." + methodGroup.Key.Name + (targetTypes.Count() > 1 ? " <- [*** TARGET MULTI-DISPATCH ***]" : ""));

                    foreach(var targetType in targetTypes)
                    {
                        report.WriteLine("    " + (targetType != null ? targetType.ToString() : "(null)"));
                    }
                }
            }

            report.WriteLine();
            report.WriteLine("Max usage count by type: " + maxUsageCountByType);
        }

    }
}

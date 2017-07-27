using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Pixel3D.Serialization.BuiltIn.DelegateHandling
{
    public static class MulticastDelegateExtensions
    {
        #region Method Creation

        private static Func<MulticastDelegate, InvocationList> CreateGetInvocationListSafe()
        {
            Trace.WriteLine("\nWARNING: Using fallback invocation getter!\n");
            Debug.Assert(false); // In development: Warn if we don't match the CLR

            return new Func<MulticastDelegate, InvocationList>((md) =>
            {
                var invocationList = md.GetInvocationList();
                return new InvocationList(invocationList, invocationList.Length);
            });
        }

        private static Func<MulticastDelegate, InvocationList> CreateGetInvocationListDirect()
        {
            // NOTE: Pulling private fields out of the CLR is blatently unsafe. So there's some safety checks and a fallback.
            try
            {
                const BindingFlags privateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

                Type multicastDelegateType = typeof(MulticastDelegate);
                FieldInfo invocationListField = multicastDelegateType.GetField("_invocationList", privateBinding);
                FieldInfo invocationCountField = multicastDelegateType.GetField("_invocationCount", privateBinding);

                // Ensure the fields are correct and available:
                if(invocationListField == null
                        || invocationListField.FieldType != typeof(object)
                        || invocationCountField == null
                        || invocationCountField.FieldType != typeof(IntPtr))
                {
                    return CreateGetInvocationListSafe();
                }


                Type invocationListType = typeof(InvocationList);
                ConstructorInfo invListMultiConstructor = invocationListType.GetConstructor(privateBinding, null, new[] { typeof(object[]), typeof(Int32) }, null);
                ConstructorInfo invListSingleConstructor = invocationListType.GetConstructor(privateBinding, null, new[] { multicastDelegateType.BaseType }, null);

                DynamicMethod dynamicMethod = new DynamicMethod("GetInvocationListDirect",
                        invocationListType, new[] { multicastDelegateType }, true);

                #region Generate IL
                {
                    ILGenerator il = dynamicMethod.GetILGenerator();
                    Label handleNotMulticast = il.DefineLabel();
                    il.DeclareLocal(typeof(object[]));

                    // object[] invocationList = d._invocationList as object[];
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, invocationListField);
                    il.Emit(OpCodes.Isinst, typeof(object[]));
                    il.Emit(OpCodes.Stloc_0);

                    // if(invocationList != null)
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Brfalse_S, handleNotMulticast);

                    //     return new InvocationList(invocationList, d._invocationCount.ToInt32());
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldflda, invocationCountField);
                    il.Emit(OpCodes.Call, typeof(IntPtr).GetMethod("ToInt32", new Type[0]));
                    il.Emit(OpCodes.Newobj, invocationListType.GetConstructor(privateBinding, null, new[] { typeof(object[]), typeof(Int32) }, null));
                    il.Emit(OpCodes.Ret);

                    // else
                    il.MarkLabel(handleNotMulticast);

                    //     return new InvocationList(d);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Newobj, invocationListType.GetConstructor(privateBinding, null, new[] { multicastDelegateType }, null));
                    il.Emit(OpCodes.Ret);
                }
                #endregion


                var getInvocationListDirect = (Func<MulticastDelegate, InvocationList>)dynamicMethod.CreateDelegate(typeof(Func<MulticastDelegate, InvocationList>));


                // Now perform a functional test to ensure the method actually does what it's supposed to!
                MulticastDelegate d1 = new Func<int>(() => 1);
                MulticastDelegate d2 = new Func<int>(() => 2);
                MulticastDelegate md = (MulticastDelegate)MulticastDelegate.Combine(d1, d2);

                // Test on a multicast delegate:
                var invocationList = getInvocationListDirect(md);
                if(invocationList.Count != 2 || invocationList[0] != d1 || invocationList[1] != d2)
                    return CreateGetInvocationListSafe();

                // Test on a single-cast delegate:
                invocationList = getInvocationListDirect(d1);
                if(invocationList.Count != 1 || invocationList[0] != d1)
                    return CreateGetInvocationListSafe();


                return getInvocationListDirect; // Success
            }
            catch(Exception) // Something exploded, give up
            {
                return CreateGetInvocationListSafe();
            }
        }

        #endregion

        static readonly Func<MulticastDelegate, InvocationList> getInvocationListDirectMethod = CreateGetInvocationListDirect();

        public static InvocationList GetInvocationListDirect(this MulticastDelegate md)
        {
            return getInvocationListDirectMethod(md);
        }
    }
}

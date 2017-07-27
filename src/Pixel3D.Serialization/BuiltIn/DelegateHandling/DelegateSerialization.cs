using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Pixel3D.Serialization.Context;
using Pixel3D.Serialization.MethodProviders;
using Pixel3D.Serialization.Static;

namespace Pixel3D.Serialization.BuiltIn.DelegateHandling
{
    static class DelegateSerialization
    {
        // NOTE: Do *not* mark these as [CustomFieldSerializer]
        //       They are accessed directly by the serializer and generator (via DelegateFieldMethodProvider)


        public static void SerializeDelegateField<T>(SerializeContext context, BinaryWriter bw, T d) where T : class // MulticastDelegate constraint not allowed by C#
        {
            SerializeDelegateField(context, bw, d as MulticastDelegate, typeof(T));
        }

        public static void DeserializeDelegateField<T>(DeserializeContext context, BinaryReader br, ref T d) where T : class // MulticastDelegate constraint not allowed by C#
        {
            MulticastDelegate mulitcastDelegate = d as MulticastDelegate;
            DeserializeDelegateField(context, br, ref mulitcastDelegate, typeof(T));
            d = mulitcastDelegate as T;
        }


        internal static void SerializeDelegateField(SerializeContext context, BinaryWriter bw, MulticastDelegate d, Type delegateType)
        {
            Debug.Assert(delegateType.BaseType == typeof(MulticastDelegate));

            if(d == null)
            {
                bw.Write((Int32)0);
                return;
            }

            InvocationList invocationList = d.GetInvocationListDirect();
            bw.WriteSmallInt32(invocationList.Count);

            foreach(var invocation in invocationList)
            {
                SerializeDelegate(context, bw, invocation, delegateType);
            }
        }

        internal static void DeserializeDelegateField(DeserializeContext context, BinaryReader br, ref MulticastDelegate d, Type delegateType)
        {
            Debug.Assert(delegateType.BaseType == typeof(MulticastDelegate));

            int multicastCount = br.ReadSmallInt32();

            if(multicastCount == 0)
            {
                d = null;
            }
            else if(multicastCount == 1)
            {
                Delegate singleDelegate = null;
                DeserializeDelegate(context, br, ref singleDelegate, delegateType);
                d = (MulticastDelegate)singleDelegate;
            }
            else
            {
                Delegate[] multicastList = new Delegate[multicastCount]; // TODO: no allocation version
                for(int i = 0; i < multicastList.Length; i++)
                {
                    DeserializeDelegate(context, br, ref multicastList[i], delegateType);
                }
                d = (MulticastDelegate)Delegate.Combine(multicastList);
            }
        }


        
        private static void SerializeDelegate(SerializeContext context, BinaryWriter bw, Delegate d, Type delegateType)
        {
            Debug.Assert(d != null);
            Debug.Assert(StaticDelegateTable.delegateTypeTable.ContainsKey(delegateType));

            DelegateTypeInfo delegateTypeInfo = StaticDelegateTable.delegateTypeTable[delegateType];

            int methodId = delegateTypeInfo.GetIdForMethod(d.Method);
            bw.Write(methodId);
            DelegateMethodInfo delegateMethodInfo = delegateTypeInfo.GetMethodInfoForId(methodId);

            if(delegateMethodInfo.canHaveTarget)
            {
                object target = d.Target;
                if(context.Walk(target))
                {
                    // Serialize type ID and target type
                    //
                    // TODO: Many delegate targets don't actually require dynamic dispatch! (especially lambdas, which *always* target a sealed classes)
                    //
                    // At the moment we go through regular dispatch (and fill the dispatch table with all potential delegate targets)
                    // because it's quick and easy (development time pressure), and because delegates require dynamic dispatch in these cases:
                    //  - The few delegates where the same method has multiple possible target types
                    //  - As per normal dispatch, where the delegate initialization is on a type that could potentially be an instance of a derived type at runtime
                    StaticDispatchTable.SerializationDispatcher(context, bw, target);
                }
            }
            else
            {
                Debug.Assert(d.Target == null);
            }
        }


        private static void DeserializeDelegate(DeserializeContext context, BinaryReader br, ref Delegate d, Type delegateType)
        {
            Debug.Assert(StaticDelegateTable.delegateTypeTable.ContainsKey(delegateType));

            DelegateTypeInfo delegateTypeInfo = StaticDelegateTable.delegateTypeTable[delegateType];
            DelegateMethodInfo delegateMethodInfo = delegateTypeInfo.GetMethodInfoForId(br.ReadInt32());

            object target = null;
            if(delegateMethodInfo.canHaveTarget)
            {
                if(context.Walk(ref target))
                    target = StaticDispatchTable.DeserializationDispatcher(context, br); // (see comment in the SerializeDelegate method)
            }

            d = Delegate.CreateDelegate(delegateType, target, delegateMethodInfo.method);
        }




        #region Method Provider

        public static SerializationMethodProviders CreateSerializationMethodProviders()
        {
            return new SerializationMethodProviders(
                    new EmptyMethodProvider(),
                    new EmptyMethodProvider(),
                    new EmptyMethodProvider(),
                    new EmptyMethodProvider(),
                    new DelegateFieldMethodProvider(typeof(DelegateSerialization).GetMethod("SerializeDelegateField")),
                    new DelegateFieldMethodProvider(typeof(DelegateSerialization).GetMethod("DeserializeDelegateField")),
                    new EmptyMethodProvider());
        }

        internal class DelegateFieldMethodProvider : MethodProvider
        {
            // Expects the methods in SerializeArray, sorted by array rank:
            internal DelegateFieldMethodProvider(MethodInfo serializeMethod)
            {
                this.serializeMethod = serializeMethod;
            }

            MethodInfo serializeMethod;

            public override MethodInfo GetMethodForType(Type type)
            {
                if(type.BaseType != typeof(MulticastDelegate))
                    return null;
                else
                    return serializeMethod.MakeGenericMethod(type);
            }
        }

        #endregion


    }
}

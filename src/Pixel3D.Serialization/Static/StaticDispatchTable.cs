using System;
using System.Collections.Generic;
using System.IO;
using Pixel3D.Serialization.Context;
using Pixel3D.Serialization.Generator;

namespace Pixel3D.Serialization.Static
{
    // Members get set by Serializer.BeginStaticInitialize's thread
    internal class StaticDispatchTable
    {
        // Field serializers ***directly*** use this table to do dispatch, which means
        // that only the static serializer methods can do dispatch without failing horribly!
        // (Basically: don't run any serializer methods except the official static ones)
        //
        // Unlike the static serializer methods themselves, we don't do any locking around this
        // (because no one should be accessing it until the serializer methods are generated anyway)

        internal static Dictionary<Type, SerializeDispatchDelegate> serializeDispatchTable; 

        // Just to save having to inline the IL code for table lookup:
        public static void SerializationDispatcher(SerializeContext context, BinaryWriter bw, object obj)
        {
            serializeDispatchTable[obj.GetType()].Invoke(context, bw, obj);
        }



        // Delegate deserializer directly uses this table:
        internal static DeserializeDispatchDelegate deserializeDispatchDelegate;

        public static object DeserializationDispatcher(DeserializeContext context, BinaryReader br)
        {
            return deserializeDispatchDelegate(context, br);
        }
    }
}

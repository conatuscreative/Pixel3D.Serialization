using System;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    static class SerializeString
    {
        [CustomFieldSerializer]
        public static void SerializeField(SerializeContext context, BinaryWriter bw, String value)
        {
            if(!context.Walk(value)) // null check
                return;

            context.VisitObject(value);
            bw.Write(value);
            context.LeaveObject();
        }

        [CustomFieldSerializer]
        public static void DeserializeField(DeserializeContext context, BinaryReader br, ref String value)
        {
            if(!context.Walk(ref value))
                return;

            value = br.ReadString(); // TODO: one day we might like to not allocate here.
            context.VisitObject(value);
        }
    }
}

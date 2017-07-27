using System;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    // In generated serializers, these are inlined.
    // In normal custom serializers, you'll probably want to inline them too.
    // But for generic custom serializers, we need to be able to provide these methods when <T> is a primitive.

    static class SerializePrimitives
    {
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Boolean value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Byte    value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref SByte   value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Int16   value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref UInt16  value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Int32   value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref UInt32  value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Int64   value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref UInt64  value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Char    value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Double  value) { bw.Write(value); }
        [CustomSerializer] public static void Serialize(SerializeContext context, BinaryWriter bw, ref Single  value) { bw.Write(value); }

        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Boolean value) { value = br.ReadBoolean(); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Byte    value) { value = br.ReadByte   (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref SByte   value) { value = br.ReadSByte  (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Int16   value) { value = br.ReadInt16  (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref UInt16  value) { value = br.ReadUInt16 (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Int32   value) { value = br.ReadInt32  (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref UInt32  value) { value = br.ReadUInt32 (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Int64   value) { value = br.ReadInt64  (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref UInt64  value) { value = br.ReadUInt64 (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Char    value) { value = br.ReadChar   (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Double  value) { value = br.ReadDouble (); }
        [CustomSerializer] public static void Deserialize(DeserializeContext context, BinaryReader br, ref Single  value) { value = br.ReadSingle (); }
    }
}

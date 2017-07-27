﻿using System.IO;
using Pixel3D.Serialization.Context;
using Pixel3D.Serialization.Static;

namespace Pixel3D.Serialization
{
    public static class Field
    {
        public static void Serialize<T>(SerializeContext context, BinaryWriter bw, ref T obj) { FieldSerializerCache<T>.Serialize(context, bw, ref obj); }
        public static void Deserialize<T>(DeserializeContext context, BinaryReader br, ref T obj) { FieldDeserializerCache<T>.Deserialize(context, br, ref obj); }
    }
}

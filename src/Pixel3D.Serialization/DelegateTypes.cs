using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization
{
    // Field methods that work for both value and reference types (so they can be used successfully in generics)

    public delegate void FieldSerializeMethod<T>(SerializeContext context, BinaryWriter bw, ref T obj);
    public delegate void FieldDeserializeMethod<T>(DeserializeContext context, BinaryReader br, ref T obj);

    // These are the actual method signatures used by the serializer:

    public delegate void ValueTypeSerializeMethod<T>(SerializeContext context, BinaryWriter bw, ref T obj) where T : struct;
    public delegate void ValueTypeDeserializeMethod<T>(DeserializeContext context, BinaryReader br, ref T obj) where T : struct;

    public delegate void ReferenceTypeSerializeMethod<T>(SerializeContext context, BinaryWriter bw, T obj) where T : class;
    public delegate void ReferenceTypeDeserializeMethod<T>(DeserializeContext context, BinaryReader br, T obj) where T : class;

    public delegate void ReferenceFieldSerializeMethod<T>(SerializeContext context, BinaryWriter bw, T obj) where T : class;
    public delegate void ReferenceFieldDeserializeMethod<T>(DeserializeContext context, BinaryReader br, ref T obj) where T : class;

    public delegate T ReferenceTypeInitializeMethod<T>() where T : class;
}

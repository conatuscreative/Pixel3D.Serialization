namespace Pixel3D.Serialization.Static
{
    // These classes offload the caching of serialization methods onto the CLR
    // (They will be filled when the CLR JITs any method that references them)
    // 
    // IMPORTANT: Do not add static constructors to these types, as this will remove their "beforefieldinit" flag
    //            (which, in turn, will cause an initialization check to occur every time they are accessed)
    
    public static class FieldSerializerCache<T>
    {
        public static readonly FieldSerializeMethod<T> Serialize = Serializer.StaticMethodLookup.GetFieldSerializeDelegate<T>();
    }

    public static class FieldDeserializerCache<T>
    {
        public static readonly FieldDeserializeMethod<T> Deserialize = Serializer.StaticMethodLookup.GetFieldDeserializeDelegate<T>();
    }

    public static class ReferenceTypeSerializerCache<T> where T : class
    {
        public static readonly ReferenceTypeSerializeMethod<T> Serialize = Serializer.StaticMethodLookup.GetReferenceTypeSerializeDelegate<T>();
    }

    public static class ReferenceTypeDeserializerCache<T> where T : class
    {
        public static readonly ReferenceTypeDeserializeMethod<T> Deserialize = Serializer.StaticMethodLookup.GetReferenceTypeDeserializeDelegate<T>();
    }

}

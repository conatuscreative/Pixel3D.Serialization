using System;

namespace Pixel3D.Serialization
{
    /// <summary>
    /// Indicates to serialization generation that this is a root type to begin searching from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class SerializationRootAttribute : Attribute { }


    // TODO: Document these:

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CustomSerializerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CustomInitializerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CustomFieldSerializerAttribute : Attribute
    {
    }


    // TODO: Make this work for auto-properties?
    /// <summary>The serializer should ignore the field</summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SerializationIgnoreAttribute : Attribute
    {
    }

    /// <summary>The serializer should ignore any delegates instantiated by this method (the delegates created should never be stored in a serialized field).</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class SerializationIgnoreDelegatesAttribute : Attribute
    {
    }

    /// <summary>All delegate types that can be serialized must be tagged with this attribute.</summary>
    [AttributeUsage(AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
    public sealed class SerializableDelegateAttribute : Attribute
    {
    }
    
    // TODO: This attribute is not yet used:
    ///// <summary>Apply to types that can be serialized as their System.Type</summary>
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface
    //        | AttributeTargets.Delegate | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    //public sealed class SerializedAsTypeAttribute : Attribute
    //{
    //}
}

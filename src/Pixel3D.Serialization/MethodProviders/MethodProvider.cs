using System;
using System.Reflection;

namespace Pixel3D.Serialization.MethodProviders
{
    abstract class MethodProvider
    {
        public abstract MethodInfo GetMethodForType(Type type);

        public MethodInfo this[Type type]
        {
            get { return GetMethodForType(type); }
        }
    }
}

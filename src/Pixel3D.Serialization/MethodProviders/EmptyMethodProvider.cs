using System;
using System.Reflection;

namespace Pixel3D.Serialization.MethodProviders
{
    class EmptyMethodProvider : MethodProvider
    {
        public override MethodInfo GetMethodForType(Type type)
        {
            return null;
        }
    }
}

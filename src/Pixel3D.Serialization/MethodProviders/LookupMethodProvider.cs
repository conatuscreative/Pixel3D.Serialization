using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Pixel3D.Serialization.MethodProviders
{
    // Generic to support lookups on MethodBuilder
    class LookupMethodProvider : MethodProvider
    {
        public LookupMethodProvider()
        {
            this.lookup = new Dictionary<Type, MethodInfo>();
        }

        public LookupMethodProvider(Dictionary<Type, MethodInfo> lookup)
        {
            this.lookup = lookup;
        }

        public readonly Dictionary<Type, MethodInfo> lookup;

        public override MethodInfo GetMethodForType(Type type)
        {
            MethodInfo method;

            if(lookup.TryGetValue(type, out method))
                return method;

            if(type.IsGenericType)
            {
                if(lookup.TryGetValue(type.GetGenericTypeDefinition(), out method))
                {
                    Debug.Assert(method.IsGenericMethodDefinition);
                    return method.MakeGenericMethod(type.GetGenericArguments());
                }
            }

            return null;
        }

        public void Add(Type type, MethodInfo method)
        {
            lookup.Add(type, method);
        }
    }
}

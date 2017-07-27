using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Pixel3D.Serialization.Discovery.ReadIL
{
    static class ResolveExtensions
    {
        /// <summary>Resolve a method from within an existing method</summary>
        public static MethodBase ResolveMethodFromMethod(this MethodBase method, int metadataToken)
        {
            return method.Module.ResolveMethod(metadataToken,
                    method.DeclaringType.GetGenericArguments(),
                    method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes);
        }

        /// <summary>Resolve a field from within an existing method</summary>
        public static FieldInfo ResolveFieldFromMethod(this MethodBase method, int metadataToken)
        {
            return method.Module.ResolveField(metadataToken,
                    method.DeclaringType.GetGenericArguments(),
                    method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes);
        }



        // These next two methods are for performance optimisation:

        /// <summary>Resolve a method from within an existing method</summary>
        public static MethodBase ResolveMethodFromMethod(this MethodBase method, int metadataToken, Type methodDeclaringType)
        {
            return method.Module.ResolveMethod(metadataToken,
                    methodDeclaringType.GetGenericArguments(),
                    method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes);
        }

        /// <summary>Resolve a field from within an existing method</summary>
        public static FieldInfo ResolveFieldFromMethod(this MethodBase method, int metadataToken, Type methodDeclaringType)
        {
            return method.Module.ResolveField(metadataToken,
                    methodDeclaringType.GetGenericArguments(),
                    method.IsGenericMethod ? method.GetGenericArguments() : Type.EmptyTypes);
        }
    }
}

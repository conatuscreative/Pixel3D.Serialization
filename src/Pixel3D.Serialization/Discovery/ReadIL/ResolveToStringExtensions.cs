using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Pixel3D.Serialization.Discovery.ReadIL
{
    static class ResolveToStringExtensions
    {
        // TODO: one day it would be nice to spit out something that ILASM might accept.

        public static string GetLocalNameFor(this MethodBase fromMethod, MemberInfo memberInfo)
        {
            string declaringAssemblyString = (memberInfo.Module != fromMethod.Module ?
                    "[" + memberInfo.Module.Assembly.GetName().Name + "]" : string.Empty);

            string declaringTypeString = (memberInfo.DeclaringType != null && memberInfo.DeclaringType != fromMethod.DeclaringType ?
                    memberInfo.DeclaringType.ToString() + ":: " : string.Empty);

            return declaringAssemblyString + declaringTypeString + memberInfo.ToString();
        }


        public static string ResolveMethodToString(this MethodBase method, int metadataToken)
        {
            return method.GetLocalNameFor(method.Module.ResolveMethod(metadataToken));
        }

        public static string ResolveFieldToString(this MethodBase method, int metadataToken)
        {
            return method.GetLocalNameFor(method.Module.ResolveField(metadataToken));
        }

        public static string ResolveTypeToString(this MethodBase method, int metadataToken)
        {
            return method.GetLocalNameFor(method.Module.ResolveType(metadataToken));
        }

    }
}

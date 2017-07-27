using System.Collections.Generic;
using System.Reflection;

namespace Pixel3D.Serialization.BuiltIn.DelegateHandling
{
    internal class DelegateTypeInfo
    {
        /// <param name="methodInfoList">Must be in network-safe order!</param>
        internal DelegateTypeInfo(List<DelegateMethodInfo> methodInfoList)
        {
            this.methodInfoList = methodInfoList;
            this.methodIdLookup = new Dictionary<MethodInfo, int>();

            for(int i = 0; i < methodInfoList.Count; i++)
            {
                methodIdLookup.Add(methodInfoList[i].method, i);
            }
        }

        Dictionary<MethodInfo, int> methodIdLookup;
        internal List<DelegateMethodInfo> methodInfoList;

        internal int GetIdForMethod(MethodInfo methodInfo)
        {
            return methodIdLookup[methodInfo];
        }

        internal DelegateMethodInfo GetMethodInfoForId(int methodId)
        {
            return methodInfoList[methodId];
        }
    }
}

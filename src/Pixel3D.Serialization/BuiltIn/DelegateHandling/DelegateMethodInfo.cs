using System.Reflection;

namespace Pixel3D.Serialization.BuiltIn.DelegateHandling
{
    internal struct DelegateMethodInfo
    {
        public DelegateMethodInfo(MethodInfo method, bool canHaveTarget)
        {
            this.method = method;
            this.canHaveTarget = canHaveTarget;
        }

        public readonly MethodInfo method;
        public readonly bool canHaveTarget;
    }
}
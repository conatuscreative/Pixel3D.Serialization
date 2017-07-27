using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Pixel3D.Serialization.Discovery.ReadIL;

namespace Pixel3D.Serialization.Discovery
{
    /// <summary>Represents the instantiation of a delegate.</summary>
    internal struct DelegateUsage
    {
        /// <summary>The static field type of the delegate Target at the instantiation site, or null for no target</summary>
        public Type targetType;
        public MethodInfo delegateMethod;
        public Type delegateType;
    }


    internal struct DelegateUsageInternal
    {
        public MethodBase instantiatingMethod;
        public long instantiationILOffset;

        public bool targetTypeKnown;
        public Type targetType;
        public MethodInfo delegateMethod;
        public Type delegateType;


        public void WriteInfo(StreamWriter writer, bool writeInstantiation = false)
        {
            string delegateTargetName = targetTypeKnown ? (targetType != null ? instantiatingMethod.GetLocalNameFor(targetType) : "(null)") : "*** UNKNOWN TARGET! ***";
            string delegateMethodName = delegateMethod != null ? instantiatingMethod.GetLocalNameFor(delegateMethod) : "*** UNKNOWN METHOD! ***";
            string delegateTypeName = instantiatingMethod.GetLocalNameFor(delegateType);

            if(writeInstantiation)
                writer.WriteLine("  at IL offset " + instantiationILOffset + " in method " + instantiatingMethod + " in type " + instantiatingMethod.DeclaringType);

            writer.WriteLine("    T = " + delegateTargetName);
            writer.WriteLine("    M = " + delegateMethodName);
            writer.WriteLine("    D = " + delegateTypeName);
        }
    }
}

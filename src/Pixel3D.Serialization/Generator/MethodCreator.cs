using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.Generator
{
    abstract class MethodCreator
    {
        public abstract MethodInfo CreateMethod(Type owner, string name, Type returnType, Type[] parameterTypes);
    }


    class DynamicMethodCreator : MethodCreator
    {
        public override MethodInfo CreateMethod(Type owner, string name, Type returnType, Type[] parameterTypes)
        {
            if(owner != null && owner.IsInterface)
                owner = null;
            if(owner == null || owner.IsArray)
                return new DynamicMethod(name, returnType, parameterTypes, true);
            else
                return new DynamicMethod(name, returnType, parameterTypes, owner, true);
        }
    }


    class MethodBuilderCreator : MethodCreator
    {
        private TypeBuilder typeBuilder;

        public MethodBuilderCreator(TypeBuilder typeBuilder)
        {
            this.typeBuilder = typeBuilder;
        }

        public override MethodInfo CreateMethod(Type owner, string name, Type returnType, Type[] parameterTypes)
        {
            const MethodAttributes staticMethod = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
            MethodBuilder mb = typeBuilder.DefineMethod(name, staticMethod, returnType, parameterTypes);

            // Take a rough guess at parameter names:
            // (Note: parameter 0 for DefineParameter is 'this', even for static methods)
            if(parameterTypes.Length > 0 && (parameterTypes[0] == typeof(SerializeContext) || parameterTypes[0] == typeof(DeserializeContext)))
                mb.DefineParameter(1, 0, "context");
            if(parameterTypes.Length > 1)
            {
                if(parameterTypes[1] == typeof(BinaryWriter))
                    mb.DefineParameter(2, 0, "bw");
                else if(parameterTypes[1] == typeof(BinaryReader))
                    mb.DefineParameter(2, 0, "br");
            }
            if(parameterTypes.Length > 2)
            {
                mb.DefineParameter(3, 0, "obj"); // The subject, probably.
            }

            return mb;
        }
    }
}

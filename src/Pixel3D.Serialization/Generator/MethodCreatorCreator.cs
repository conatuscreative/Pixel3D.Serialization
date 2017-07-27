using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Pixel3D.Serialization.Generator
{
    abstract class MethodCreatorCreator // <- yes, I went there.
    {
        public abstract MethodCreator Create(string containingTypeName);
    }


    class DynamicMethodCreatorCreator : MethodCreatorCreator
    {
        public override MethodCreator Create(string containingTypeName)
        {
            return new DynamicMethodCreator();
        }
    }


    class MethodBuilderCreatorCreator : MethodCreatorCreator
    {
        ModuleBuilder moduleBuilder;
        string @namespace;

        public MethodBuilderCreatorCreator(ModuleBuilder moduleBuilder, string @namespace)
        {
            this.moduleBuilder = moduleBuilder;
            this.@namespace = @namespace;
        }


        List<TypeBuilder> typeBuilders = new List<TypeBuilder>();

        public void Finish()
        {
            foreach(var typeBuilder in typeBuilders)
            {
                typeBuilder.CreateType();
            }
        }


        public override MethodCreator Create(string containingTypeName)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(@namespace + "." + containingTypeName);
            typeBuilders.Add(typeBuilder);
            return new MethodBuilderCreator(typeBuilder);
        }

    }
}

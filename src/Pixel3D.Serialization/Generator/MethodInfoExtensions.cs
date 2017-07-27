using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace Pixel3D.Serialization.Generator
{
    static class MethodInfoExtensions
    {
        // Let's pretend that MethodInfo provides GetILGenerator as an abstract method...
        public static ILGenerator GetILGenerator(this MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = methodInfo as DynamicMethod;
            if(dynamicMethod != null)
                return dynamicMethod.GetILGenerator();

            MethodBuilder methodBuilder = methodInfo as MethodBuilder;
            if(methodBuilder != null)
                return methodBuilder.GetILGenerator();

            throw new InvalidOperationException("Cannot generate IL for fixed MethodInfo");
        }
    }
}

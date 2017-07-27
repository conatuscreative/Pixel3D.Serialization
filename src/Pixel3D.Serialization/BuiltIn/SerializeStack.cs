using System.Collections.Generic;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    static class SerializeStack
    {
        [CustomSerializer]
        public static void Serialize<T>(SerializeContext context, BinaryWriter bw, Stack<T> stack)
        {
            context.VisitObject(stack);

            bw.WriteSmallInt32(stack.Count);
            foreach(var entry in stack)
            {
                T item = entry;
                Field.Serialize(context, bw, ref item);
            }

            context.LeaveObject();
        }

        [CustomSerializer]
        public static void Deserialize<T>(DeserializeContext context, BinaryReader br, Stack<T> stack)
        {
            context.VisitObject(stack);

            int count = br.ReadSmallInt32();

            stack.Clear();

            for(int i = 0; i < count; i++)
            {
                T item = default(T);
                Field.Deserialize(context, br, ref item);
                stack.Push(item);
            }
        }

        [CustomInitializer]
        public static Stack<T> Initialize<T>()
        {
            return new Stack<T>();
        }

    }
}

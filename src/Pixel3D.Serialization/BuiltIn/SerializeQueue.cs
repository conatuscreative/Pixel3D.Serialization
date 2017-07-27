using System.Collections.Generic;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    static class SerializeQueue
    {
        [CustomSerializer]
        public static void Serialize<T>(SerializeContext context, BinaryWriter bw, Queue<T> queue)
        {
            context.VisitObject(queue);

            bw.WriteSmallInt32(queue.Count);
            foreach(var entry in queue)
            {
                T item = entry;
                Field.Serialize(context, bw, ref item);
            }

            context.LeaveObject();
        }

        [CustomSerializer]
        public static void Deserialize<T>(DeserializeContext context, BinaryReader br, Queue<T> queue)
        {
            context.VisitObject(queue);

            int count = br.ReadSmallInt32();

            queue.Clear();

            for(int i = 0; i < count; i++)
            {
                T item = default(T);
                Field.Deserialize(context, br, ref item);
                queue.Enqueue(item);
            }
        }

        [CustomInitializer]
        public static Queue<T> Initialize<T>()
        {
            return new Queue<T>();
        }

    }
}

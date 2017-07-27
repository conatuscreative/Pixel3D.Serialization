using System.Collections.Generic;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    static class SerializeList
    {
        [CustomSerializer]
        public static void Serialize<T>(SerializeContext context, BinaryWriter bw, List<T> list)
        {
            context.VisitObject(list);

            bw.WriteSmallInt32(list.Count);
            for(int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                Field.Serialize(context, bw, ref item);
            }

            context.LeaveObject();
        }

        [CustomSerializer]
        public static void Deserialize<T>(DeserializeContext context, BinaryReader br, List<T> list)
        {
            context.VisitObject(list);

            int count = br.ReadSmallInt32();

            list.Clear();
            if(list.Capacity < count)
                list.Capacity = count;

            for(int i = 0; i < count; i++)
            {
                T item = default(T);
                Field.Deserialize(context, br, ref item);
                list.Add(item);
            }
        }

        [CustomInitializer]
        public static List<T> Initialize<T>()
        {
            return new List<T>();
        }
        
    }
}

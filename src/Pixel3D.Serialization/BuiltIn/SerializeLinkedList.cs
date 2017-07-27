using System;
using System.Collections.Generic;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    static class SerializeLinkedList
    {
        [CustomSerializer]
        public static void Serialize<T>(SerializeContext context, BinaryWriter bw, LinkedList<T> list)
        {
            context.VisitObject(list);

            bw.WriteSmallInt32(list.Count); // O(1)
            foreach(var entry in list)
            {
                var item = entry;
                Field.Serialize(context, bw, ref item);
            }

            context.LeaveObject();
        }

        [CustomSerializer]
        public static void Deserialize<T>(DeserializeContext context, BinaryReader br, LinkedList<T> list)
        {
            context.VisitObject(list);

            int count = br.ReadSmallInt32();

            list.Clear();

            for(int i = 0; i < count; i++)
            {
                T item = default(T);
                Field.Deserialize(context, br, ref item);
                list.AddLast(item);
            }
        }

        [CustomInitializer]
        public static LinkedList<T> Initialize<T>()
        {
            return new LinkedList<T>();
        }




        [CustomSerializer]
        public static void Serialize<T>(SerializeContext context, BinaryWriter bw, LinkedListNode<T> value)
        {
            throw new NotSupportedException("Cannot serialize a linked list node directly");
        }

        [CustomSerializer]
        public static void Deserialize<T>(DeserializeContext context, BinaryReader br, LinkedListNode<T> value)
        {
            throw new NotSupportedException("Cannot serialize a linked list node directly");
        }
    }
}

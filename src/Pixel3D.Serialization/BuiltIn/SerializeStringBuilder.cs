using System.Text;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.BuiltIn
{
    class SerializeStringBuilder
    {
        [CustomFieldSerializer]
        public static void SerializeField(SerializeContext context, BinaryWriter bw, StringBuilder stringBuilder)
        {
            if(!context.Walk(stringBuilder)) // null check
                return;

            context.VisitObject(stringBuilder);

            bw.WriteSmallInt32(stringBuilder.Length);
            for(int i = 0; i < stringBuilder.Length; i++)
            {
                bw.Write(stringBuilder[i]);
            }

            context.LeaveObject();
        }

        [CustomFieldSerializer]
        public static void DeserializeField(DeserializeContext context, BinaryReader br, ref StringBuilder stringBuilder)
        {
            if(!context.Walk(ref stringBuilder))
                return;

            int length = br.ReadSmallInt32();
            stringBuilder = new StringBuilder(length);
            stringBuilder.Length = length;
            context.VisitObject(stringBuilder);
            for(int i = 0; i < length; i++)
            {
                stringBuilder[i] = br.ReadChar();
            }
        }
    }
}

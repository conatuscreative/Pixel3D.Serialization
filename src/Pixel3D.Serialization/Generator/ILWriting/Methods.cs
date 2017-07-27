using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Pixel3D.Serialization.Context;
using Pixel3D.Serialization.MethodProviders;
using Pixel3D.Serialization.Static;
using System.Runtime.Serialization;

namespace Pixel3D.Serialization.Generator.ILWriting
{
    // Provide cached access to methods used in generated IL
    internal static class Methods
    {
        public static readonly MethodInfo StaticDispatchTable_SerializationDispatcher = typeof(StaticDispatchTable).GetMethod("SerializationDispatcher");

        public static readonly MethodInfo SerializeContext_VisitObject = typeof(SerializeContext).GetMethod("VisitObject", new[] { typeof(object) });
        public static readonly MethodInfo SerializeContext_LeaveObject = typeof(SerializeContext).GetMethod("LeaveObject", Type.EmptyTypes);
        public static readonly MethodInfo DeserializeContext_VisitObject = typeof(DeserializeContext).GetMethod("VisitObject", new[] { typeof(object) });

        public static readonly MethodInfo SerializeContext_Walk = typeof(SerializeContext).GetMethod("Walk", new[] { typeof(object) });


#if DEBUG
        public static readonly MethodInfo SerializeContext_DebugTrace = typeof(SerializeContext).GetMethod("DebugTrace", new[] { typeof(string) });
        public static readonly MethodInfo DeserializeContext_DebugTrace = typeof(DeserializeContext).GetMethod("DebugTrace", new[] { typeof(string) });
#endif


        public static readonly MethodInfo Type_GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
        public static readonly MethodInfo MethodBase_GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });
        public static readonly MethodInfo FormatterServices_GetUninitializedObject = typeof(FormatterServices).GetMethod("GetUninitializedObject");


        public static readonly MethodInfo BinaryWriter_WriteInt32 = typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Int32) });
        public static readonly MethodInfo BinaryReader_ReadInt32 = typeof(BinaryReader).GetMethod("ReadInt32");
        public static readonly MethodInfo BinaryWriter_WriteByte = typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Byte) });
        public static readonly MethodInfo BinaryReader_ReadByte = typeof(BinaryReader).GetMethod("ReadByte");

        public static readonly LookupMethodProvider BinaryWriterPrimitive = new LookupMethodProvider(new Dictionary<Type, MethodInfo>
        {
            { typeof(Boolean), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Boolean) }) },
            { typeof(Byte   ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Byte   ) }) },
            { typeof(SByte  ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(SByte  ) }) },
            { typeof(Int16  ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Int16  ) }) },
            { typeof(UInt16 ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(UInt16 ) }) },
            { typeof(Int32  ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Int32  ) }) },
            { typeof(UInt32 ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(UInt32 ) }) },
            { typeof(Int64  ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Int64  ) }) },
            { typeof(UInt64 ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(UInt64 ) }) },
            { typeof(Char   ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Char   ) }) },
            { typeof(Double ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Double ) }) },
            { typeof(Single ), typeof(BinaryWriter).GetMethod("Write", new Type[] { typeof(Single ) }) },
        });

        public static readonly LookupMethodProvider BinaryReaderPrimitive = new LookupMethodProvider(new Dictionary<Type, MethodInfo>
        {
            { typeof(Boolean), typeof(BinaryReader).GetMethod("ReadBoolean") },
            { typeof(Byte   ), typeof(BinaryReader).GetMethod("ReadByte")    },
            { typeof(SByte  ), typeof(BinaryReader).GetMethod("ReadSByte")   },
            { typeof(Int16  ), typeof(BinaryReader).GetMethod("ReadInt16")   },
            { typeof(UInt16 ), typeof(BinaryReader).GetMethod("ReadUInt16")  },
            { typeof(Int32  ), typeof(BinaryReader).GetMethod("ReadInt32")   },
            { typeof(UInt32 ), typeof(BinaryReader).GetMethod("ReadUInt32")  },
            { typeof(Int64  ), typeof(BinaryReader).GetMethod("ReadInt64")   },
            { typeof(UInt64 ), typeof(BinaryReader).GetMethod("ReadUInt64")  },
            { typeof(Char   ), typeof(BinaryReader).GetMethod("ReadChar")    },
            { typeof(Double ), typeof(BinaryReader).GetMethod("ReadDouble")  },
            { typeof(Single ), typeof(BinaryReader).GetMethod("ReadSingle")  },
        });

    }
}

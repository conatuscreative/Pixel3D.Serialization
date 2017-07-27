using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Pixel3D.Serialization.Context;

namespace Pixel3D.Serialization.Generator
{
    internal delegate void SerializeDispatchDelegate(SerializeContext context, BinaryWriter bw, object obj);
    internal delegate object DeserializeDispatchDelegate(DeserializeContext context, BinaryReader br);
}

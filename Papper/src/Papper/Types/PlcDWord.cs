﻿using Papper.Internal;
using System;
using System.Buffers.Binary;

namespace Papper.Types
{
    internal class PlcDWord : PlcObject
    {
        // Use share size for this datatype, we will never change the size
        private static readonly PlcSize _size = new PlcSize { Bytes = 4 };
        public override Type DotNetType => typeof(uint);


        public PlcDWord(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
            => data.IsEmpty ? default : BinaryPrimitives.ReadUInt32BigEndian(data.Slice(plcObjectBinding.Offset));


        public override void ConvertToRaw(object value, PlcObjectBinding plcObjectBinding, Span<byte> data)
            => BinaryPrimitives.WriteUInt32BigEndian(data.Slice(plcObjectBinding.Offset), Convert.ToUInt32(value));
    }
}

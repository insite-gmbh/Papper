using Papper.Internal;
using System;
using System.Buffers.Binary;

namespace Papper.Types
{
    internal class PlcLWord : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 8 };
        public override Type DotNetType => typeof(ulong);

        public PlcLWord(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => data.IsEmpty ? default : BinaryPrimitives.ReadUInt64BigEndian(data.Slice(plcObjectBinding.Offset));


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
         => BinaryPrimitives.WriteUInt64BigEndian(data.Slice(plcObjectBinding.Offset), Convert.ToUInt64(value));
    }
}

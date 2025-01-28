using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcInt : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 2 };
        public override Type DotNetType => typeof(short);

        public PlcInt(string name) : base(name)
            => Size = _size;


        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
            => data.IsEmpty ? default : BinaryPrimitives.ReadInt16BigEndian(data.Slice(plcObjectBinding.Offset));

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
            => BinaryPrimitives.WriteInt16BigEndian(data.Slice(plcObjectBinding.Offset), Convert.ToInt16(value, CultureInfo.InvariantCulture));
    }
}

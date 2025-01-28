using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcWord : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 2 };
        public override Type DotNetType => typeof(ushort);

        public PlcWord(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
            => data.IsEmpty ? default : BinaryPrimitives.ReadUInt16BigEndian(data[plcObjectBinding.Offset..]);

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
            => BinaryPrimitives.WriteUInt16BigEndian(data[plcObjectBinding.Offset..], Convert.ToUInt16(value, CultureInfo.InvariantCulture));
    }
}

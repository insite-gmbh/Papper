using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcDInt : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 4 };
        public override Type DotNetType => typeof(int);

        public PlcDInt(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
            => data.IsEmpty ? default : BinaryPrimitives.ReadInt32BigEndian(data.Slice(plcObjectBinding.Offset));

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
            => BinaryPrimitives.WriteInt32BigEndian(data.Slice(plcObjectBinding.Offset), Convert.ToInt32(value, CultureInfo.InvariantCulture));
    }
}

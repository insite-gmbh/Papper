using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcLReal : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 8 };
        public override Type DotNetType => typeof(double);

        public PlcLReal(string name) : base(name)
            => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => data.IsEmpty ? default : BinaryPrimitives.ReadDoubleBigEndian(data[plcObjectBinding.Offset..]);


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
         => BinaryPrimitives.WriteDoubleBigEndian(data[plcObjectBinding.Offset..], Convert.ToDouble(value, CultureInfo.InvariantCulture));
    }
}

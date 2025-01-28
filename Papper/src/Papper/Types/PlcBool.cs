using Papper.Internal;
using System;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcBool : PlcObject
    {
        // Use share size for this datatype, we will never change the size
        private static readonly PlcSize _size = new() { Bits = 1 };

        public override Type DotNetType => typeof(bool);


        public PlcBool(string name) :
            base(name)
        {
            Size = _size;
            AllowOddByteOffsetInArray = true;
        }

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (data.IsEmpty)
            {
                return false;
            }

            var baseOffset = plcObjectBinding.Offset + (plcObjectBinding.MetaData.Offset.Bits) / 8;
            var bit = plcObjectBinding.Offset + plcObjectBinding.MetaData.Offset.Bits - baseOffset;
            return data[baseOffset].GetBit(bit);
        }

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var baseOffset = plcObjectBinding.Offset + (plcObjectBinding.MetaData.Offset.Bits) / 8;
            var bit = plcObjectBinding.Offset + plcObjectBinding.MetaData.Offset.Bits - baseOffset;
            data[baseOffset] = data[baseOffset].SetBit(bit, Convert.ToBoolean(value, CultureInfo.InvariantCulture));
        }

        public void AssigneOffsetFrom(int bitoffset)
        {
            var offsetBits = bitoffset;
            var offsetByte = offsetBits / 8;
            offsetBits -= offsetByte * 8;

            Offset.Bytes += offsetByte;
            Offset.Bits += offsetBits;
        }

    }
}

using Papper.Internal;
using System;

namespace Papper.Types
{
    internal class PlcSInt : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 1 };
        public override Type DotNetType => typeof(sbyte);

        public PlcSInt(string name) :
            base(name)
        {
            Size = _size;
            AllowOddByteOffsetInArray = true;
        }

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
            => data.IsEmpty ? default : unchecked((sbyte)data[plcObjectBinding.Offset]);


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (value is sbyte sb)
            {
                data[plcObjectBinding.Offset] = (byte)unchecked(sb);
            }
            else
            {
                data[plcObjectBinding.Offset] = (byte)unchecked(0);
            }
        }

    }
}

using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcTime : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 4 };

        private static readonly TimeSpan _minValue = TimeSpan.FromMilliseconds(Int32.MinValue);
        private static readonly TimeSpan _maxValue = TimeSpan.FromMilliseconds(Int32.MaxValue);

        public override Type DotNetType => typeof(TimeSpan);

        public PlcTime(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => data.IsEmpty ? TimeSpan.Zero : TimeSpan.FromMilliseconds(BinaryPrimitives.ReadInt32BigEndian(data[plcObjectBinding.Offset..]));

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (value is TimeSpan ts)
            {
                if(ts < _minValue)
                {
                    ts = _minValue;
                }
                else if(ts > _maxValue)
                {
                    ts = _maxValue;
                }

                BinaryPrimitives.WriteInt32BigEndian(data[plcObjectBinding.Offset..], 
                                                     Convert.ToInt32(ts.TotalMilliseconds));
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(data[plcObjectBinding.Offset..],
                                     Convert.ToInt32(TimeSpan.Zero.TotalMilliseconds));
            }
        }
    }
}

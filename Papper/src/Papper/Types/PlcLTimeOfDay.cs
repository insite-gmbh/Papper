using Papper.Internal;
using System;
using System.Buffers.Binary;

namespace Papper.Types
{
    internal class PlcLTimeOfDay : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 8 };
        private static readonly TimeSpan _minValue = TimeSpan.Zero;
        private static readonly TimeSpan _maxValue = new(TimeSpan.TicksPerDay - 1);

        public override Type DotNetType => typeof(TimeSpan);

        public PlcLTimeOfDay(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
               => data.IsEmpty ? TimeSpan.Zero : new TimeSpan(BinaryPrimitives.ReadInt64BigEndian(data[plcObjectBinding.Offset..]) / 100);

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
                BinaryPrimitives.WriteInt64BigEndian(data[plcObjectBinding.Offset..], 
                                                     Convert.ToInt64(ts.Ticks * 100));
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(data[plcObjectBinding.Offset..],
                                                    Convert.ToInt64(TimeSpan.Zero.Ticks * 100));
            }
        }
    }
}

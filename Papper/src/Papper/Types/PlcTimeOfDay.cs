using Papper.Internal;
using System;
using System.Buffers.Binary;

namespace Papper.Types
{
    internal class PlcTimeOfDay : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 4 };
        private static readonly TimeSpan _minValue = TimeSpan.Zero;
        private static readonly TimeSpan _maxValue = new(TimeSpan.TicksPerDay - 1);

        public override Type DotNetType => typeof(TimeSpan);

        public PlcTimeOfDay(string name) : base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => data.IsEmpty ? TimeSpan.MinValue : TimeSpan.FromMilliseconds(BinaryPrimitives.ReadUInt32BigEndian(data[plcObjectBinding.Offset..]));


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (value is TimeSpan ts)
            {
                if (ts < _minValue)
                {
                    ts = _minValue;
                }
                else if (ts > _maxValue)
                {
                    ts = _maxValue;
                }

                BinaryPrimitives.WriteUInt32BigEndian(data[plcObjectBinding.Offset..],
                                                             Convert.ToUInt32(ts.TotalMilliseconds));
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(data[plcObjectBinding.Offset..],
                                                             Convert.ToUInt32(TimeSpan.Zero.TotalMilliseconds));
            }
        
        } 
    }
}

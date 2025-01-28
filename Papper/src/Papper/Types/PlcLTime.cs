using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Globalization;

namespace Papper.Types
{
    internal class PlcLTime : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 8 };
        private static readonly TimeSpan _minValue = AddNanoseconds(TimeSpan.ParseExact("106751:23:47:16.854", @"d\:hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.AssumeNegative), -775808);
        private static readonly TimeSpan _maxValue = AddNanoseconds(TimeSpan.ParseExact("106751:23:47:16.854", @"d\:hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None), 775807);

        public override Type DotNetType => typeof(TimeSpan);
        public PlcLTime(string name) :
            base(name)
         => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
               => data.IsEmpty ? TimeSpan.MinValue : new TimeSpan(BinaryPrimitives.ReadInt64BigEndian(data[plcObjectBinding.Offset..]) / 100);

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

                BinaryPrimitives.WriteInt64BigEndian(data[plcObjectBinding.Offset..],
                                                     Convert.ToInt64(ts.Ticks * 100));
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(data[plcObjectBinding.Offset..],
                                                    Convert.ToInt64(TimeSpan.Zero.Ticks * 100));
            }
        }

        private static TimeSpan AddNanoseconds(TimeSpan self, int nanoseconds)
        {
            const int nanosecondsPerTick = 100;
            return self.Add(TimeSpan.FromTicks((int)Math.Round(nanoseconds / (double)nanosecondsPerTick)));
        }
    }
}

using Papper.Internal;
using System;
using System.Buffers.Binary;

namespace Papper.Types
{
    internal class PlcDateTimeL : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 12 };
        private static readonly DateTime _minValue = new(1970, 01, 01, 00, 00, 00, 0);
        private static readonly DateTime _maxValue = AddNanoseconds(new DateTime(2262, 04, 11, 23, 47, 16), 854775807);
        public override Type DotNetType => typeof(DateTime);

        public PlcDateTimeL(string name) : base(name)
            => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (data.IsEmpty)
            {
                return _minValue;
            }
            var source = data[plcObjectBinding.Offset..];
            var year = (int)BinaryPrimitives.ReadUInt16BigEndian(source);
            var nanoseconds = (int)(BinaryPrimitives.ReadUInt32BigEndian(source[8..]));
            if (year >= 1970 && year <= 2262)
            {
                try
                {
                    return AddNanoseconds(new DateTime(year,
                                           source[2],
                                           source[3],
                                           source[5],
                                           source[6],
                                           source[7],
                                           DateTimeKind.Unspecified),
                                           nanoseconds);
                }
                catch (ArgumentOutOfRangeException)
                { 
                }
            }
            return _minValue;
        }


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var dt = value is DateTime d ? d : _minValue;

            if(dt < _minValue)
            {
                dt = _minValue;
            }
            else if(dt > _maxValue)
            {
                dt = _maxValue;
            }


            var destination = data[plcObjectBinding.Offset..];
            BinaryPrimitives.WriteUInt16BigEndian(destination, (ushort)dt.Year);
            destination[2] = (byte)dt.Month;
            destination[3] = (byte)dt.Day;
            destination[4] = (byte)(dt.DayOfWeek + 1); // 1(Sunday) to 7(Saturday)
            destination[5] = (byte)dt.Hour;
            destination[6] = (byte)dt.Minute;
            destination[7] = (byte)dt.Second;
            BinaryPrimitives.WriteUInt32LittleEndian(destination[8..], (uint)((dt.Millisecond * 1000000) + GetNanoseconds(dt)));
        }



        private static int GetNanoseconds(DateTime self)
        {
            const int nanosecondsPerTick = 100;
            return (int)(self.Ticks % TimeSpan.TicksPerMillisecond) * nanosecondsPerTick;
        }

        private static DateTime AddNanoseconds(DateTime self, int nanoseconds)
        {
            const int nanosecondsPerTick = 100;
            return self.AddTicks((int)Math.Round(nanoseconds / (double)nanosecondsPerTick));
        }

    }
}

using Papper.Internal;
using System;

namespace Papper.Types
{
    internal class PlcDateTime : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 8 };
        private static readonly DateTime _minValue = new(1970, 01, 01, 00, 00, 00, 0);
        private static readonly DateTime _maxValue = new(2089, 12, 31, 23, 59, 59, 999);
        public override Type DotNetType => typeof(DateTime);


        public PlcDateTime(string name) :
            base(name) => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (data.IsEmpty)
            {
                return _minValue;
            }

            int bt = data[plcObjectBinding.Offset];
            //BCD conversion
            bt = (((bt >> 4)) * 10) + ((bt & 0x0f));
            var year = bt < 90 ? 2000 : 1900;  //  BCD#90 = 1990 ; BCD#0 = 2000; BCD#89 = 2089

            year += bt;

            //month
            bt = data[plcObjectBinding.Offset + 1];
            var month = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //day
            bt = data[plcObjectBinding.Offset + 2];
            var day = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //hour
            bt = data[plcObjectBinding.Offset + 3];
            var hour = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //minute
            bt = data[plcObjectBinding.Offset + 4];
            var minute = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //sec
            bt = data[plcObjectBinding.Offset + 5];
            var sec = (((bt >> 4)) * 10) + ((bt & 0x0f));

            //millisecond
            //Byte 6 BCD + MSB (Byte 7)
            bt = data[plcObjectBinding.Offset + 6];
            int bt1 = data[plcObjectBinding.Offset + 7];
            var milli = (((bt >> 4)) * 10) + ((bt & 0x0f));
            milli = milli * 10 + (bt1 >> 4);

            //weekday
            //LSB (Byte 7) 1=Sunday
            //bt = b[pos + 7];
            //weekday = (bt1 & 0x0f); 
            if (year > 0 && month > 0 && month <= 12 && day > 0 && hour >= 0 && hour <= 24 && minute >= 0 && minute < 60 && sec >= 0 && sec < 60)
            {
                try
                {
                    return new DateTime(year, month, day, hour, minute, sec, milli, DateTimeKind.Local);
                }
                catch (Exception) { }
            }
            return _minValue;
        }

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var dateTime = value is DateTime dt ? dt : _minValue;

            if (dateTime < _minValue)
            {
                dateTime = _minValue;
            }
            else if (dateTime > _maxValue)
            {
                dateTime = _maxValue;
            }


            var tmp = dateTime.Year - ((dateTime.Year / 100) * 100);
            data[plcObjectBinding.Offset] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = dateTime.Month;
            data[plcObjectBinding.Offset + 1] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = dateTime.Day;
            data[plcObjectBinding.Offset + 2] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = dateTime.Hour;
            data[plcObjectBinding.Offset + 3] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = dateTime.Minute;
            data[plcObjectBinding.Offset + 4] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = dateTime.Second;
            data[plcObjectBinding.Offset + 5] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = dateTime.Millisecond / 10;
            var rem = dateTime.Millisecond % 10;
            data[plcObjectBinding.Offset + 6] = Convert.ToByte((tmp / 10) << 4 | tmp % 10);

            tmp = (int)dateTime.DayOfWeek;
            data[plcObjectBinding.Offset + 7] = Convert.ToByte((rem) << 4 | tmp % 10);

        }
    }
}

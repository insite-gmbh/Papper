using Papper.Internal;
using System;

namespace Papper.Types
{
    internal class PlcS5Time : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 2 };

        private static readonly TimeSpan _minValue = TimeSpan.Zero;
        private static readonly TimeSpan _maxValue = new(2, 46, 30, 0);


        public override Type DotNetType => typeof(TimeSpan);
        public PlcS5Time(string name) :
            base(name) => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (data.IsEmpty)
            {
                return TimeSpan.Zero;
            }
            var w1 = data[plcObjectBinding.Offset + 1].GetBcdByte();
            var idx0Value = data[plcObjectBinding.Offset];
            var w2 = ((idx0Value & 0x0f));
            long number = w2 * 100 + w1;
            switch ((idx0Value >> 4) & 0x03)
            {
                case 0:
                    number *= 100000;
                    break;
                case 1:
                    number *= 1000000;
                    break;
                case 2:
                    number *= 10000000;
                    break;
                case 3:
                    number *= 100000000;
                    break;

            }

            return new TimeSpan(number);  // Is this really correct?
        }

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var time = value is TimeSpan ts ? ts : TimeSpan.Zero;

            if(time < _minValue)
            {
                time = _minValue;
            }
            else if(time > _maxValue)
            {
                time = _maxValue;
            }



            byte valueBase;
            int val;

            if (time.TotalMilliseconds <= 999 * 10)
            {
                valueBase = 0;
                val = Convert.ToInt32(time.TotalMilliseconds) / 10;
            }
            else if (time.TotalMilliseconds <= 999 * 100)
            {
                valueBase = 1;
                val = Convert.ToInt32(time.TotalMilliseconds) / 100;
            }
            else if (time.TotalMilliseconds <= 999 * 1000)
            {
                valueBase = 2;
                val = Convert.ToInt32(time.TotalMilliseconds) / 1000;
            }
            else if (time.TotalMilliseconds <= 999 * 10000)
            {
                valueBase = 3;
                val = Convert.ToInt32(time.TotalMilliseconds) / 10000;
            }
            else
            {
                valueBase = 3;
                val = 999;
            }

            var p3 = (val / 100);
            var p2 = ((val - p3 * 100) / 10);
            var p1 = (val - p3 * 100 - p2 * 10);

            data[plcObjectBinding.Offset] = Convert.ToByte(valueBase << 4 | p3);
            data[plcObjectBinding.Offset + 1] = Convert.ToByte((p2 << 4 | p1));
        }
    }
}

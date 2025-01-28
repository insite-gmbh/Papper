using Papper.Internal;
using System;
using System.Buffers.Binary;

namespace Papper.Types
{
    internal class PlcLDateTime : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 8 };
        private static readonly DateTime _minValue = new(1970, 01, 01, 00, 00, 00, 000);
        private static readonly DateTime _maxValue = AddNanoseconds(new DateTime(2262, 04, 11, 23, 47, 16), 854775807);
        public override Type DotNetType => typeof(DateTime);

        public PlcLDateTime(string name) : base(name)
            => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
            => data.IsEmpty ? _minValue : _minValue.AddTicks(BinaryPrimitives.ReadInt64BigEndian(data[plcObjectBinding.Offset..]) / 100);

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (value is DateTime dt)
            {
                if(dt < _minValue)
                {
                    dt = _minValue;
                }
                else if(dt > _maxValue)
                {
                    dt = _maxValue;
                }
                BinaryPrimitives.TryWriteInt64BigEndian(data[plcObjectBinding.Offset..], (dt.Ticks - _minValue.Ticks) * 100);
            }
            else
            {
                BinaryPrimitives.TryWriteInt64BigEndian(data[plcObjectBinding.Offset..], (_minValue.Ticks) * 100);
            }
        }

        private static DateTime AddNanoseconds(DateTime self, int nanoseconds)
        {
            const int nanosecondsPerTick = 100;
            return self.AddTicks((int)Math.Round(nanoseconds / (double)nanosecondsPerTick));
        }
    }
}

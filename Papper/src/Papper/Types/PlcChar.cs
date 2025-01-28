using Papper.Internal;
using System;
using System.Globalization;
using System.Linq;

namespace Papper.Types
{
    internal class PlcChar : PlcObject
    {
        // Use share size for this data type, we will never change the size
        private static readonly PlcSize _size = new() { Bytes = 1 };
        public override Type DotNetType => typeof(char);

        public PlcChar(string name) :
            base(name)
        {
            Size = _size;
            AllowOddByteOffsetInArray = true;
        }

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => data.IsEmpty ? default : Convert.ToChar(data[plcObjectBinding.Offset]);


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data) => data[plcObjectBinding.Offset] = value is string s ? Convert.ToByte(s.FirstOrDefault()) : Convert.ToByte(value, CultureInfo.InvariantCulture);
    }
}

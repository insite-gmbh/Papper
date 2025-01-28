using Papper.Internal;
using System;
using System.Buffers.Binary;
using System.Text;

namespace Papper.Types
{
    internal class PlcWString : PlcObject, ISupportStringLengthAttribute
    {
        private const int _defaultStringLength = 254;
        private const char _defaultFillChar = '\0';
        private readonly byte _defaultFillByte;
        private readonly PlcSize _size = new();

        public override Type DotNetType => typeof(string);


        public int StringLength
        {
            get => _size.Bytes;
            set => _size.Bytes = (value * 2) + 4;
        }

        public override PlcSize? Size
        {
            get => _size;
            protected set {; }
        }


        public PlcWString(string name)
            : base(name)
        {
            _defaultFillByte = Convert.ToByte(_defaultFillChar);
            StringLength = _defaultStringLength;
        }

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (data.IsEmpty)
            {
                return string.Empty;
            }

            var maxLength = BinaryPrimitives.ReadInt16BigEndian(data[plcObjectBinding.Offset..]);
            var curLength = BinaryPrimitives.ReadInt16BigEndian(data[(plcObjectBinding.Offset + 2)..]);
            var take = Math.Min(Math.Min(maxLength, curLength), Size == null ? 0 : Size.Bytes - 4) * 2;
            return Encoding.BigEndianUnicode.GetString(data.Slice(plcObjectBinding.Offset + 4, take).ToArray());
        }

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var maxLength = Size == null ? (short)0 : Convert.ToInt16((Size.Bytes / 2) - 2);
            var i = plcObjectBinding.Offset;
            BinaryPrimitives.TryWriteInt16BigEndian(data.Slice(i, 2), maxLength);
            i += 2;

            var fill = string.Empty;
            if (value != null)
            {
                var str = value.ToString() ?? string.Empty;
                var curLength = Convert.ToInt16(str.Length);
                var take = Math.Min(maxLength, curLength);
                fill = str[..take];

                BinaryPrimitives.TryWriteInt16BigEndian(data.Slice(i, 2), take);
                i += 2;


                foreach (var c in Encoding.BigEndianUnicode.GetBytes(fill))
                {
                    data[i++] = c;
                }
            }
            else
            {
                BinaryPrimitives.TryWriteInt16BigEndian(data.Slice(i, 2), 0);  //currentLength
                i += 2;
            }

            for (var j = 0; j < (maxLength - fill.Length) * 2; j++)
            {
                data[i + j] = _defaultFillByte;
            }

        }


        public void AssigneLengthFrom(ISupportStringLengthAttribute s)
        {
            if (s != null)
            {
                _size.Bytes = s.StringLength;
            }
        }

    }
}

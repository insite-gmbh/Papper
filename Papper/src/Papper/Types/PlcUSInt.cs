﻿using System;
using System.Linq;
using Papper.Internal;

namespace Papper.Types
{
    internal class PlcUSInt : PlcObject
    {
        public PlcUSInt(string name) :  base(name)
        {
            Size = new PlcSize {Bytes = 1};
            AllowOddByteOffsetInArray = true;
        }

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (data.IsEmpty)
                return default;

            return data[plcObjectBinding.Offset];
        }

        public override void ConvertToRaw(object value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var s = value as string;
            data[plcObjectBinding.Offset] = s != null ? Convert.ToByte(s.FirstOrDefault()) : Convert.ToByte(value);
        }
    }
}

﻿using System;
using System.Linq;
using Papper.Internal;

namespace Papper.Types
{
    internal class PlcInt : PlcObject
    {
        public PlcInt(string name) : 
            base(name )
        {
            Size = new PlcSize { Bytes = 2 };
        }

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, byte[] data)
        {
            if (data == null || !data.Any())
                return default;

            return data.GetSwap<short>(plcObjectBinding.Offset);
        }

        public override void ConvertToRaw(object value, PlcObjectBinding plcObjectBinding, byte[]  data)
        {
            var subset = Convert.ToInt16(value).SetSwap();
            for (var i = 0; i < subset.Length; i++)
                data[plcObjectBinding.Offset + i] = subset[i];
        }
    }
}

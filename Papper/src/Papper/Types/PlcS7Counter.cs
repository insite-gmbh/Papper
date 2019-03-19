﻿using Papper.Internal;
using System;

namespace Papper.Types
{
    internal class PlcS7Counter : PlcObject
    {
        // Use share size for this datatype, we will never change the size
        private static readonly PlcSize _size = new PlcSize { Bytes = 2 };
        public override Type DotNetType => typeof(int);

        public PlcS7Counter(string name) :
            base(name) => Size = _size;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => (data == null || data.IsEmpty) ? default : data.GetBcdWord(plcObjectBinding.Offset);

        public override void ConvertToRaw(object value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var subset = Convert.ToInt32(value).SetBcdWord();
            for (var i = 0; i < subset.Length; i++)
                data[plcObjectBinding.Offset + i] = subset[i];
        }
    }
}

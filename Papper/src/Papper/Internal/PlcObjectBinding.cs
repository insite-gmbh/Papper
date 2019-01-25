﻿using Papper.Types;
using System;

namespace Papper.Internal
{
    internal class PlcObjectBinding
    {
        public PlcObjectBinding(PlcRawData rawData, PlcObject metaData, int offset, int validationTimeMs, bool fullType = false)
        {
            RawData = rawData;
            MetaData = metaData;
            Offset = offset;
            ValidationTimeInMs = validationTimeMs;
            FullType = fullType;
        }

        public bool IsActive { get; set; }
        public int ValidationTimeInMs { get; set; }

        public bool FullType { get; private set; }

        public Memory<byte> Data
        {
            get { return RawData.ReadDataCache; }
        }

        public DateTime LastUpdate
        {
            get { return RawData.LastUpdate; }
        }

        public PlcRawData RawData { get; }

        public PlcObject MetaData { get; }

        public int Offset { get; private set; }
        public int Size { get { return MetaData.Size.Bytes; } }

        public object ConvertFromRaw(Span<byte> data)
        {
            return MetaData.ConvertFromRaw(this, data);
        }

        public void ConvertToRaw(object obj, Span<byte> data)
        {
            MetaData.ConvertToRaw(obj, this, data);
        }

        public Type GetMetaType()
        {
            return MetaData.GetType();
        }
    }
}

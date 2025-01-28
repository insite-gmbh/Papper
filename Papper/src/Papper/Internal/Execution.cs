using Papper.Types;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace Papper.Internal
{
    /// <summary>
    /// Holds the execution operation
    /// </summary>
    internal class Execution(PlcRawData plcRawData, Dictionary<string, PlcObjectBinding> bindings, int validationTimeMS, bool symbolicAccess)
    {
        private readonly bool _symbolicAccess = symbolicAccess;

        public PlcRawData PlcRawData { get; private set; } = plcRawData;
        public Dictionary<string, PlcObjectBinding> Bindings { get; private set; } = bindings;
        public int ValidationTimeMs { get; private set; } = validationTimeMS;
        public ExecutionResult ExecutionResult { get; private set; }

        public DateTime LastChange { get; private set; } = DateTime.MaxValue;


        /// <summary>
        /// This method is called to handle save the read results in the execution.
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public Execution ApplyDataPack(DataPack pack)
        {
            if (pack.ExecutionResult == ExecutionResult.Ok)
            {
                if (pack is DataPackAbsolute aadp)
                {
                    Memory<byte> cache = Memory<byte>.Empty;

                    if (PlcRawData.ReadDataCache is Memory<byte> rdc)
                    {
                        cache = rdc;
                    }

                    var ts = pack.Timestamp;
                    if (cache.IsEmpty || !cache.Span.SequenceEqual(aadp.Data.Span))
                    {
                        if (ts > LastChange || LastChange == DateTime.MaxValue)
                        {
                            LastChange = ts; // We detected a change in this data area 
                        }
                    }

                    if (ts > PlcRawData.LastUpdate)
                    {
                        lock (PlcRawData)
                        {
                            if (ts > PlcRawData.LastUpdate)
                            {
                                PlcRawData.ReadDataCache = aadp.Data;
                                PlcRawData.LastUpdate = ts;
                            }
                        }
                    }
                    
                }
                else if(pack is DataPackSymbolic sadp)
                {
                    var ts = pack.Timestamp;
                    if (PlcRawData.ReadDataCache == null || PlcRawData.ReadDataCache != sadp.Value)
                    {
                        if (ts > LastChange || LastChange == DateTime.MaxValue)
                        {
                            LastChange = ts; // We detected a change in this data area 
                        }
                    }

                    if (ts > PlcRawData.LastUpdate)
                    {
                        lock (PlcRawData)
                        {
                            if (ts > PlcRawData.LastUpdate)
                            {
                                PlcRawData.ReadDataCache = sadp.Value;
                                PlcRawData.LastUpdate = ts;
                            }
                        }
                    }
                }
            }
            ExecutionResult = pack.ExecutionResult;
            return this;
        }

        /// <summary>
        /// After a write we can invalidate the data area, so the subscriber reads before the validation time is over
        /// </summary>
        public void Invalidate() => PlcRawData.LastUpdate = DateTime.MinValue;


        public KeyValuePair<Execution, DataPack> CreateReadDataPack()
        {
            if (_symbolicAccess)
            {
                return new KeyValuePair<Execution, DataPack>(this, new DataPackSymbolic
                {
                    Type = PlcRawData.DataType,
                    SymbolicName = PlcRawData.SymbolicAccessName,
                    Selector = PlcRawData.Selector
                });
            }
            else
            {
                return new KeyValuePair<Execution, DataPack>(this, new DataPackAbsolute
                {
                    Selector = PlcRawData.Selector,
                    Offset = PlcRawData.Offset,
                    Length = PlcRawData.Size > 0 ? PlcRawData.Size : 1
                });
            }
        }



        public KeyValuePair<Execution, IEnumerable<DataPack>> CreateWriteDataPack(Dictionary<string, object> values, Dictionary<PlcRawData, byte[]>? memoryBuffer)
        {
            var res = Bindings.Where(x => !x.Value.MetaData.IsReadOnly)
                               .Select(x => new KeyValuePair<string, IEnumerable<DataPack>>(x.Key, Create(values[x.Key], x.Value, memoryBuffer, x.Value.Offset, _symbolicAccess)))
                               .SelectMany(x => x.Value)
                               .ToList();
            return new KeyValuePair<Execution, IEnumerable<DataPack>>(this, res);
        }

        private static List<DataPack> Create(object value, PlcObjectBinding binding, Dictionary<PlcRawData, byte[]>? memoryBuffer, int dataOffset, bool symbolicAccess)
        {
            static byte[] GetOrCreateBufferAndApplyValue(PlcObjectBinding plcBinding, Dictionary<PlcRawData, byte[]> dict, object value)
            {
                if (!dict.TryGetValue(plcBinding.RawData, out var buffer))
                {
                    buffer = ArrayPool<byte>.Shared.Rent(plcBinding.RawData.MemoryAllocationSize);
                    dict.Add(plcBinding.RawData, buffer);
                }


                if (value is byte[] b && plcBinding.Size == b.Length)
                {
                    // we got raw data for the type, so we need not to convert them
                    b.CopyTo(buffer, plcBinding.Offset);
                }
                else
                {
                    plcBinding.ConvertToRaw(value, buffer);
                }
                return buffer;
            }

            (var begin, var end) = CreateBitMasks(binding);
            var data = memoryBuffer != null ? GetOrCreateBufferAndApplyValue(binding, memoryBuffer, value) : null;


            return [.. binding.RawData.WriteSlots.Select<Slot,DataPack>(slot =>
            {
                if (symbolicAccess)
                {
                    var res = new DataPackSymbolic
                    {
                        SymbolicName = binding.RawData.SymbolicAccessName,
                        Type = binding.RawData.DataType,
                        Value = value,
                        Selector = binding.RawData.Selector
                    };
                    return res;
                }
                else
                {
                    var res = new DataPackAbsolute
                    {
                        Selector = binding.RawData.Selector,
                        Offset = binding.Offset + slot.Offset,
                        Length = Math.Min(slot.Size, GetSize(binding)),
                        BitMaskBegin = slot.Mask != 0xff ? slot.Mask : begin,
                        BitMaskEnd = slot.Mask != 0xff ? (byte)0xFF : end,
                        Data = data
                    };
                    res.Data = res.Data.Slice(dataOffset + slot.Offset - binding.RawData.Offset, res.Length);
                    return res;
                }
            })];

        }


        private static int GetSize(PlcObjectBinding binding)
        {
            if (binding.MetaData is PlcBool)
            {
                return 1;
            }
            else if (binding.MetaData is PlcArray plcArray && plcArray.ArrayType is PlcBool)
            {
                return plcArray.Size.Bytes + (plcArray.Size.Bits > 0 ? 1 : 0);
            }
            return binding.Size;
        }

        private static (byte begin, byte end) CreateBitMasks(PlcObjectBinding binding)
        {
            byte begin = 0;
            byte end = 0;
            if (binding.MetaData is PlcBool plcBool)
            {
                begin = Converter.SetBit(0, plcBool.Offset.Bits, true);
            }
            else if (binding.MetaData is PlcArray plcArray && plcArray.ArrayType is PlcBool)
            {
                if (plcArray.Offset.Bits > 0 || ((plcArray.ArrayLength % 8) != 0))
                {
                    var take = 8 - plcArray.Offset.Bits;
                    foreach (var item in plcArray.Childs.Take(take).OfType<PlcObject>())
                    {
                        begin = Converter.SetBit(begin, item.Offset.Bits, true);
                    }

                    var lastItems = ((plcArray.Offset.Bits + plcArray.ArrayLength) % 8);
                    if (lastItems > 0)
                    {
                        foreach (var item in plcArray.Childs.Skip(plcArray.ArrayLength - lastItems).OfType<PlcObject>())
                        {
                            end = Converter.SetBit(end, item.Offset.Bits, true);
                        }

                        if (begin == 0)
                        {
                            begin = 0xFF;
                        }
                    }
                    else if (begin > 0)
                    {
                        end = 0xFF;
                    }
                }

            }
            return (begin, end);
        }

    }
}

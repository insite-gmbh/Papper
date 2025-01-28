using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Papper.Types
{
    internal class PlcStruct : PlcObject
    {
        public const int AlignmentInBytes = 2;
        private readonly Type _structType;

        public override Type DotNetType => _structType;

        public override PlcSize Size
        {
            get
            {
                var byteOffset = 0;
                if (Childs.Any())
                {
                    var first = Childs.OfType<PlcObject>().FirstOrDefault();
                    var last = Childs.OfType<PlcObject>().LastOrDefault();

                    if (first != null && last != null)
                    {

                        if (first != last)
                        {
                            var fixe1 = last.Size == null || last.Size.Bytes == 0;
                            byteOffset = (last.Offset.Bytes + (fixe1 ? 1 : last.Size!.Bytes)) - first.Offset.Bytes;

                            if (!fixe1 && last.BitSize > 0)
                            {
                                byteOffset += 1;
                            }
                        }
                        else
                        {
                            var fixe1 = first.Size == null || first.Size.Bytes == 0;
                            byteOffset = fixe1 ? 1 : first.Size!.Bytes;

                            if (!fixe1 && first.BitSize > 0)
                            {
                                byteOffset += 1;
                            }
                        }

                        if (byteOffset % 2 == 1)
                        {
                            byteOffset += 1;
                        }
                    }
                }

                return new PlcSize
                {
                    Bytes = ((byteOffset + AlignmentInBytes - 1) / AlignmentInBytes) * AlignmentInBytes
                };
            }
        }


        public PlcStruct(string name, Type structType)
            : base(name) => _structType = structType ?? ExceptionThrowHelper.ThrowArgumentNullException<Type>(nameof(structType));

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (!plcObjectBinding.FullType || _structType == null)
            {
                var obj = new ExpandoObject();
                foreach (var child in plcObjectBinding.MetaData.Childs.OfType<PlcObject>().AsParallel())
                {
                    var binding = new PlcObjectBinding(plcObjectBinding.RawData, child, plcObjectBinding.Offset + child.Offset.Bytes, plcObjectBinding.ValidationTimeInMs);
                    AddProperty(obj, child.Name, child.ConvertFromRaw(binding, data));
                }
                return obj;
            }
            else
            {
                var obj = Activator.CreateInstance(_structType);
                foreach (var child in plcObjectBinding.MetaData.Childs.OfType<PlcObject>().AsParallel())
                {
                    var prop = _structType.GetProperty(child.OriginName); // need origin name because the type has this name
                    var binding = new PlcObjectBinding(plcObjectBinding.RawData, child, plcObjectBinding.Offset + child.Offset.Bytes, plcObjectBinding.ValidationTimeInMs, true);
                    prop?.SetValue(obj, child.ConvertFromRaw(binding, data));
                }
                return obj!;
            }
        }

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            if (value != null)
            {
                var properties = GetKeyValuePairs(value);
                foreach (var child in plcObjectBinding.MetaData.Childs.OfType<PlcObject>())
                {
                    var binding = new PlcObjectBinding(plcObjectBinding.RawData, child, plcObjectBinding.Offset + child.Offset.Bytes, plcObjectBinding.ValidationTimeInMs);
                    if (properties.TryGetValue(child.OriginName, out var prop))
                    {
                        child.ConvertToRaw(prop, binding, data);
                    }
                }
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            var list = (parent as List<dynamic>);
            if (list != null)
            {
                list.Add(value);
            }
            else
            {
                if (parent is IDictionary<string, object> dictionary)
                {
                    dictionary[name] = value;
                }
            }
        }

        private static IDictionary<string, object?> GetKeyValuePairs(object value)
        {
            //var dyn = value as DynamicPlcObject;
            //if (dyn != null)
            //    return dyn.ToDictionary();
            var dictionary = value as IDictionary<string, object?>;
            return dictionary ?? value.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(value));
        }
    }
}

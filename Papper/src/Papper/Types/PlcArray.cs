using Papper.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Papper.Types
{
    internal class PlcArray : PlcObject
    {

        public override Type DotNetType => typeof(ExpandoObject);

        private readonly Dictionary<Type, object> _typeInstances = new();
        private readonly Dictionary<int, ITreeNode> _indexCache = new();
        private readonly PlcSize _size = new();
        private static readonly Regex _regexSplitBy = new("[\\]]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);
        private PlcObject _arrayType;
        private int _from;
        private int _to;
        public int Dimension { get; set; }

        public override bool HasReadOnlyChilds => ArrayType.HasReadOnlyChilds;
        public override bool HasNotAccessibleChilds => ArrayType.HasNotAccessibleChilds;

        public int From
        {
            get => _from;
            set
            {
                if (_from != value)
                {
                    _from = value;
                    if (_arrayType != null)
                    {
                        CalculateSize();
                    }
                }
            }
        }

        public int To
        {
            get => _to;
            set
            {
                if (_to != value)
                {
                    _to = value;
                    if (_arrayType != null)
                    {
                        CalculateSize();
                    }
                }
            }
        }

        public int ArrayLength => To - From + 1;

        public PlcObject ArrayType
        {
            get => _arrayType;
            set
            {
                if (_arrayType != value)
                {
                    _arrayType = value;
                    CalculateSize();
                }
            }
        }

        public PlcObject? LeafElementType { get; set; }

        public override PlcSize Size => _size;

        public PlcArray(string name, PlcObject arrayType, int from = 0, int to = 0)
            : base(name)
        {
            _arrayType = arrayType;
            CalculateSize();
            From = from;
            To = to;
        }


        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
        {

            var type = ArrayType.GetType();

            if (type == typeof(PlcByte) || type == typeof(PlcUSInt))
            {
                return InternalConvert<byte>(plcObjectBinding, data);
            }

            if (type == typeof(PlcBool))
            {
                return InternalConvert<bool>(plcObjectBinding, data);
            }

            if (type == typeof(PlcChar))
            {
                return InternalConvert<char>(plcObjectBinding, data);
            }

            if (type == typeof(PlcString) || type == typeof(PlcWString) || type == typeof(PlcWChar))
            {
                return InternalConvert<string>(plcObjectBinding, data);
            }

            if (type == typeof(PlcSInt))
            {
                return InternalConvert<sbyte>(plcObjectBinding, data);
            }

            if (type == typeof(PlcInt))
            {
                return InternalConvert<short>(plcObjectBinding, data);
            }

            if (type == typeof(PlcUInt) || type == typeof(PlcWord))
            {
                return InternalConvert<ushort>(plcObjectBinding, data);
            }

            if (type == typeof(PlcDInt) || type == typeof(PlcS7Counter))
            {
                return InternalConvert<int>(plcObjectBinding, data);
            }

            if (type == typeof(PlcUDInt) || type == typeof(PlcDWord))
            {
                return InternalConvert<uint>(plcObjectBinding, data);
            }

            if (type == typeof(PlcLInt))
            {
                return InternalConvert<long>(plcObjectBinding, data);
            }

            if (type == typeof(PlcULInt) || type == typeof(PlcLWord))
            {
                return InternalConvert<ulong>(plcObjectBinding, data);
            }

            if (type == typeof(PlcDate) || type == typeof(PlcDateTime) || type == typeof(PlcLDateTime) || type == typeof(PlcDateTimeL))
            {
                return InternalConvert<DateTime>(plcObjectBinding, data);
            }

            if (type == typeof(PlcS5Time) || type == typeof(PlcTime) || type == typeof(PlcTimeOfDay) || type == typeof(PlcLTime) || type == typeof(PlcLTimeOfDay))
            {
                return InternalConvert<TimeSpan>(plcObjectBinding, data);
            }

            if (type == typeof(PlcReal))
            {
                return InternalConvert<float>(plcObjectBinding, data);
            }

            if (type == typeof(PlcLReal))
            {
                return InternalConvert<double>(plcObjectBinding, data);
            }

            if (type == typeof(PlcArray))
            {
                if (plcObjectBinding.FullType && plcObjectBinding.MetaData.ElemenType != null)
                {
                    if (!_typeInstances.TryGetValue(plcObjectBinding.MetaData.ElemenType, out var instance))
                    {
                        instance = Activator.CreateInstance(plcObjectBinding.MetaData.ElemenType);
                        _typeInstances.Add(plcObjectBinding.MetaData.ElemenType, instance);
                    }
                    return InternalConvert(plcObjectBinding, instance ?? null, data, true, plcObjectBinding.MetaData.ElemenType);
                }
                return ArrayType.ConvertFromRaw(plcObjectBinding, data);
            }

            if (plcObjectBinding.FullType && plcObjectBinding.MetaData.ElemenType != null)
            {
                if (!_typeInstances.TryGetValue(plcObjectBinding.MetaData.ElemenType, out var instance))
                {
                    instance = Activator.CreateInstance(plcObjectBinding.MetaData.ElemenType);
                    if (instance != null)
                    {
                        _typeInstances.Add(plcObjectBinding.MetaData.ElemenType, instance);
                    }
                }

                return InternalConvert(plcObjectBinding, instance ?? null, data, true, plcObjectBinding.MetaData.ElemenType);
            }
            return InternalConvert<ExpandoObject>(plcObjectBinding, data);

        }


        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
        {
            var list = value as IEnumerable;
            var type = ArrayType.GetType();

            //handle byte array for Json --- TODO:  not so beautiful here
            if (list is string convert && (type == typeof(PlcByte) || ArrayType.ElemenType == typeof(byte)))
            {
                value = list = Convert.FromBase64String(convert);
            }

            if (list != null)
            {
                //Special handling for byte and char, because of performance (specially with big data)
                if (type == typeof(PlcByte) && (value is byte[] || value is Memory<byte>))
                {
                    if (value is byte[] byteArray)
                    {
                        byteArray.CopyTo(data.Slice(plcObjectBinding.Offset, byteArray.Length));
                    }
                }
                else if (type == typeof(PlcChar) && (value is char[] || value is Memory<char>))
                {
                    if (value is char[] charArray)
                    {
                        for (var i = 0; i < charArray.Length; i++)
                        {
                            data[plcObjectBinding.Offset + i] = Convert.ToByte(charArray[i]);
                        }
                    }
                }
                else
                {
                    var enumerator = list.GetEnumerator();
                    var childEnumeratore = Childs.OfType<PlcObject>().GetEnumerator();
                    for (var i = 0; i < ArrayLength; i++)
                    {
                        if (enumerator.MoveNext())
                        {
                            if (childEnumeratore.MoveNext())
                            {
                                var child = childEnumeratore.Current;
                                var binding = new PlcObjectBinding(plcObjectBinding.RawData, child, plcObjectBinding.Offset + child.Offset.Bytes + (GetElementSizeForOffset() * i), plcObjectBinding.ValidationTimeInMs);
                                ArrayType.ConvertToRaw(enumerator.Current, binding, data);
                            }
                            else
                            {
                                ExceptionThrowHelper.ThrowArrayIndexExeption(From + i);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// This method converts the given binding data to the correct representation data type
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="plcObjectBinding"></param>
        /// <param name="type">instance of target type</param>
        /// <returns></returns>
        private object InternalConvert<T>(PlcObjectBinding plcObjectBinding, T type, Span<byte> data, bool fully = false, Type? t = null)
            => InternalConvert<T>(plcObjectBinding, data, fully, t);

        /// <summary>
        /// This method converts the given binding data to the correct representation data type
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="plcObjectBinding"></param>
        /// <param name="type">instance of target type</param>
        /// <returns></returns>
        private object InternalConvert<T>(PlcObjectBinding plcObjectBinding, Span<byte> data, bool fully = false, Type? t = null)
        {

            if (!data.IsEmpty)
            {
                var type = typeof(T);
                //special handling of byte and char, because of performance (specially with big data)
                if (type == typeof(byte))
                {
                    return data.Slice(plcObjectBinding.Offset, ArrayLength).ToArray();
                }
                else if (type == typeof(char))
                {
                    var d = data.Slice(plcObjectBinding.Offset, ArrayLength).ToArray();
                    var result = new char[ArrayLength];
                    for (var i = 0; i < ArrayLength; i++)
                    {
                        result[i] = Convert.ToChar(d[i]);
                    }
                    return result;
                }
                else if (fully && t != null)
                {
                    //Special handling for object types
                    Array? list = null;
                    var idx = From;
                    var childEnumerator = Childs.OfType<PlcObject>().GetEnumerator();
                    for (var i = 0; i < ArrayLength; i++)
                    {
                        if (!childEnumerator.MoveNext())
                        {
                            ExceptionThrowHelper.ThrowArrayIndexExeption(idx);
                        }

                        var child = childEnumerator.Current;
                        var binding = new PlcObjectBinding(plcObjectBinding.RawData, child, plcObjectBinding.Offset + child.Offset.Bytes + ((idx - From) * GetElementSizeForOffset()), plcObjectBinding.ValidationTimeInMs, fully);
                        var element = ((T)ArrayType.ConvertFromRaw(binding, data));

                        if (list == null)
                        {
                            if (element is IList l)
                            {
                                list = Array.CreateInstance(l.GetType(), ArrayLength);
                            }
                            else
                            {
                                list = Array.CreateInstance(t, ArrayLength);
                            }
                        }

                        list.SetValue(element, i);
                        idx++;
                    }
                    return list ?? Array.CreateInstance(t, ArrayLength);
                }
                else
                {
                    var list = new T[ArrayLength];
                    var idx = From;
                    var childEnumerator = Childs.OfType<PlcObject>().GetEnumerator();
                    for (var i = 0; i < ArrayLength; i++)
                    {
                        if (!childEnumerator.MoveNext())
                        {
                            ExceptionThrowHelper.ThrowArrayIndexExeption(idx);
                        }

                        var child = childEnumerator.Current;
                        var binding = new PlcObjectBinding(plcObjectBinding.RawData, child, plcObjectBinding.Offset + child.Offset.Bytes + ((idx - From) * GetElementSizeForOffset()), plcObjectBinding.ValidationTimeInMs, fully);
                        list[i] = ((T)ArrayType.ConvertFromRaw(binding, data));
                        idx++;
                    }
                    return list;
                }
            }
            return new T[ArrayLength];
        }

        /// <summary>
        /// Compare two object if they are structural equal and also the value of each element of the structure
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public override bool AreDataEqual(object obj1, object obj2)
        {

            if (obj1 is IEnumerable list1 && obj2 is IEnumerable list2)
            {
                var enumerator1 = list1.GetEnumerator();
                var enumerator2 = list2.GetEnumerator();
                for (var i = 0; i < ArrayLength; i++)
                {
                    if (enumerator1.MoveNext() && enumerator2.MoveNext())
                    {
                        if (!base.AreDataEqual(enumerator1.Current, enumerator2.Current))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                return base.AreDataEqual(obj1, obj2);
            }

            return true;
        }

        /// <summary>
        /// Holds all elements of the array
        /// </summary>
        public override IEnumerable<ITreeNode> Childs
        {
            get
            {
                for (var i = From; i <= To; i++)
                {
                    var idx = GetIndex(i);
                    if (idx != null)
                    {
                        yield return idx;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the child with the given name or null if there is no child with that name
        /// </summary>
        /// <param name="name">Name of the child</param>
        /// <returns></returns>
        public override ITreeNode? GetChildByName(string name) => base.GetChildByName(name);

        /// <summary>
        /// Get a Node by it's path recursively
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset">we have to give the offset to the next element of the array (because of recursive data structures)</param>
        /// <param name="getRef"></param>
        /// <returns></returns>
        public override ITreeNode? Get(ITreePath path, ref int offset, ref StringBuilder symbolicPath, bool getRef = false)
        {
            if (path.IsPathToCurrent && !path.IsPathIndexed)
            {
                return this;
            }

            var idx = path.ArrayIndizes[0];
            if (idx >= From && idx <= To)
            {
                offset += Offset.Bytes + ((idx - From) * GetElementSizeForOffset());


                { 
                    var nullBasedIndex = From == 0 ? idx :  From > 0 ? idx - From : From + (idx * -1);
                    var symbolicAccessName = SymbolicAccessName + string.Format(CultureInfo.InvariantCulture, "[{0}]", nullBasedIndex);
                    symbolicPath.Append(".");
                    symbolicPath.Append(symbolicAccessName);
                }


                return (_indexCache.TryGetValue(idx, out var ret) ?
                            ret :
                            GetIndex(idx))?.Get(CreateSubPath(path), ref offset, ref symbolicPath);
            }
            return null;
        }

        private int GetElementSizeForOffset()
        {
            var elem = LeafElementType ?? ArrayType;
            var size = elem.Size == null ? 0 : elem.Size.Bytes;
            if (!elem.AllowOddByteOffsetInArray && size % 2 != 0)
            {
                size++;
            }

            return size;
        }

        private static ITreePath CreateSubPath(ITreePath path)
        {
            var indizes = path.ArrayIndizes.Length;
            if (indizes > 1)
            {
                var nodes = new List<string>();
                var first = path.Nodes.First();
                Match firstMatch = _regexSplitBy.Match(first);
                nodes.Add(first[(firstMatch.Index + 1)..]);
                nodes.AddRange(path.Nodes.Skip(1));
                return new PlcMetaDataTreePath(nodes.Aggregate((a, b) => a + PlcMetaDataTreePath.Separator + b));
            }
            else
            {
                return path.StepDown();
            }
        }

        public override void Accept(VisitNode visit) => base.Accept(visit);

        public void CalculateSize()
        {
            var isBoolean = _arrayType is PlcBool || _arrayType.Size == null;
            Size.Bits = isBoolean ? ArrayLength * _arrayType.Size!.Bits : 0;
            Offset.Bits = _arrayType.Offset.Bits;
            if (_arrayType is ISupportStringLengthAttribute && _arrayType.Size!.Bytes % 2 != 0)
            {
                var result = 0;
                for (var i = 0; i < ArrayLength; i++)
                {
                    if (result % 2 != 0)
                    {
                        result++;
                    }

                    result += _arrayType.Size.Bytes;
                }
                Size.Bytes = result;
            }
            else
            {
                Size.Bytes = isBoolean ? 0 : ArrayLength * _arrayType.Size!.Bytes;
            }

            if (isBoolean)
            {
                var bits = (_arrayType.Offset.Bits + Size.Bits);
                Size.Bytes = bits / 8;
                Size.Bits = bits - Size.Bytes * 8;
            }
        }

        private PlcObject? GetIndex(int idx)
        {
            lock (_indexCache)
            {
                if (_indexCache.TryGetValue(idx, out var ret) && ret is PlcObject plco)
                {
                    return plco;
                }

                var objResult = ArrayType is PlcStruct
                    ? new PlcObjectRef($"[{idx}]", ArrayType)
                    : PlcObjectFactory.CreatePlcObjectForArrayIndex(ArrayType, idx, From);
                if (objResult is PlcObject plcObj)
                {
                    plcObj.ElemenType = ElemenType;
                    plcObj.ArrayStartIndex = From;

                    var nullBasedIndex = From == 0 ? idx : From > 0 ? idx - From : From + (idx * -1);
                    plcObj.SymbolicAccessName = string.Format(CultureInfo.InvariantCulture, "[{0}]", nullBasedIndex);
                    plcObj.Parent = this;
                    _indexCache.Add(idx, plcObj);
                    return plcObj;
                }
                return null;
            }
        }

        /// <summary>
        /// This method tries to convert a string to an object, this could be used to convert a string to an array or so.
        /// Depending on the ArrayType we use the string and slit them by element (e.g. Char or Byte) or in any other case we
        /// use ';' as a separator for the elements
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object? StringToObject(string value)
        {
            try
            {
                var type = PlcObjectFactory.GetTypeForPlcObject(ArrayType.GetType());
                if (type == typeof(char))
                {
                    return value.ToCharArray();
                }
                if (type == typeof(byte))
                {
                    return value.ToArray();
                }
                var parts = value.Split(';');
                return parts.Select(part => Convert.ChangeType(value, type, CultureInfo.InvariantCulture)).ToList();
            }
            catch
            { }
            return null;
        }
    }
}

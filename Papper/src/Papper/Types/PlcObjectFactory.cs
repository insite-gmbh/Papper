using Papper.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Papper.Types
{
    internal static class PlcObjectFactory
    {
        private static readonly Dictionary<Type, Type> _typeMatch = new()
        {
            { typeof(bool), typeof(PlcBool) },
            { typeof(byte), typeof(PlcByte) },
            { typeof(sbyte), typeof(PlcSInt) },
            { typeof(short), typeof(PlcInt) },
            { typeof(int), typeof(PlcDInt) },
            { typeof(double), typeof(PlcLReal) },
            { typeof(long), typeof(PlcLInt) },
            { typeof(ushort), typeof(PlcWord) },
            { typeof(uint), typeof(PlcDWord) },
            { typeof(ulong), typeof(PlcLWord) },
            { typeof(DateTime), typeof(PlcDateTime) },
            { typeof(TimeSpan), typeof(PlcTime) },
            { typeof(string), typeof(PlcString) },
            { typeof(float), typeof(PlcReal) },
            { typeof(char), typeof(PlcChar) },
        };

        private static readonly Dictionary<Type, Type> _reverseTypeMatch = _typeMatch.ToDictionary(x => x.Value, x => x.Key);

        private static readonly Dictionary<string, Type> _typeNameMatch = new(StringComparer.OrdinalIgnoreCase)
        {
            { "S5Time", typeof(PlcS5Time) },
            { "TimeOfDay", typeof(PlcTimeOfDay) },
            { "LTimeOfDay", typeof(PlcLTimeOfDay) },
            { "Bit", typeof(PlcBool) },
            { "Bool", typeof(PlcBool) },
            { "Byte", typeof(PlcByte) },
            { "SInt", typeof(PlcSInt) },
            { "USInt", typeof(PlcUSInt) },
            { "Int", typeof(PlcInt) },
            { "DInt", typeof(PlcDInt) },
            { "UDInt", typeof(PlcUDInt) },
            { "LInt", typeof(PlcLInt) },
            { "ULInt", typeof(PlcULInt) },
            { "Word", typeof(PlcWord) },
            { "DWord", typeof(PlcDWord) },
            { "LWord", typeof(PlcLWord) },
            { "DateTime", typeof(PlcDateTime) },
            { "Date", typeof(PlcDate) },
            { "LDateTime", typeof(PlcLDateTime) },
            { "DateTimeL", typeof(PlcDateTimeL) },
            { "LDT", typeof(PlcLDateTime) },
            { "DTL", typeof(PlcDateTimeL) },
            { "Time", typeof(PlcTime) },
            { "LTime", typeof(PlcLTime) },
            { "String", typeof(PlcString) },
            { "WString", typeof(PlcWString) },
            { "Real", typeof(PlcReal) },
            { "LReal", typeof(PlcLReal) },
            { "Float", typeof(PlcReal) },
            { "Char", typeof(PlcChar) },
            { "WChar", typeof(PlcWChar) },
            { "S7Counter", typeof(PlcS7Counter) },
        };

        public static PlcObject? CreatePlcObjectFromPlcTypeName(string plcTypeName, object? value)
        {
            if (_typeNameMatch.TryGetValue(plcTypeName, out var plcType))
            {
                var plcObject = Activator.CreateInstance(plcType, plcTypeName) as PlcObject;

                if (plcObject is ISupportStringLengthAttribute s)
                {
                    if (value is string str)
                    {
                        s.StringLength = str.Length;
                    }
                    else if (value is Array a)
                    {
                        s.StringLength = a.Length - 2;
                    }
                }
                else if (plcObject is PlcArray)
                {
                    if (value is Array a && plcObject is PlcArray obj)
                    {
                        obj.From = 0;
                        obj.To = a.Length - 1;
                        obj.Dimension = 1;
                    }
                }

                return plcObject;
            }
            return null;
        }

        public static PlcObject? CreatePlcObjectFromType(Type t, object? value)
        {
            if (_typeMatch.TryGetValue(t, out var plcType))
            {
                var plcObject = Activator.CreateInstance(plcType, t.Name) as PlcObject;

                if (plcObject is ISupportStringLengthAttribute s)
                {
                    if (value is string str)
                    {
                        s.StringLength = str.Length;
                    }
                    else if (value is Array a)
                    {
                        s.StringLength = a.Length - 2;
                    }
                }
                else if (plcObject is PlcArray)
                {
                    if (value is Array a && plcObject is PlcArray obj)
                    {
                        obj.From = 0;
                        obj.To = a.Length - 1;
                        obj.Dimension = 1;
                    }
                }

                return plcObject;
            }
            return null;
        }

        public static PlcObject? CreatePlcObject(PropertyInfo pi, int? arrayIndex = null)
        {
            var plcType = GetTypeFromAttribute(pi);
            PlcObject? instance;
            PlcObject? leafPlcObject = null;
            (var name, var originName) = GetName(pi);

            if (pi.PropertyType.IsArray && arrayIndex == null)
            {
                PlcObject? element;
                var elementType = pi.PropertyType.GetElementType();
                var dimensions = 0;


                if (elementType?.IsArray == true)
                {
                    while (elementType?.IsArray == true)
                    {
                        elementType = elementType.GetElementType();
                        dimensions++;
                    }

                    if (elementType != null && _typeMatch.TryGetValue(elementType, out plcType))
                    {
                        element = Activator.CreateInstance(plcType, name) as PlcObject;
                        UpdateSize(pi, element);
                    }
                    else
                    {
                        element = Activator.CreateInstance(typeof(PlcStruct), name, elementType) as PlcObject;
                        UpdateSize(pi, element);
                    }

                    leafPlcObject = element;
                    for (var i = dimensions; i > 0; i--)
                    {
                        element = Activator.CreateInstance(typeof(PlcArray), string.Empty, element, 0, 0) as PlcObject;
                        UpdateSize(pi, element, i);
                    }
                }
                else if(plcType != null || (elementType != null && _typeMatch.TryGetValue(elementType, out plcType)))
                {
                    element = Activator.CreateInstance(plcType, name) as PlcObject;
                    UpdateSize(pi, element);
                }
                else
                {
                    element = Activator.CreateInstance(typeof(PlcStruct), name, pi.PropertyType) as PlcObject;
                }

                if (element != null)
                {
                    element.ElemenType = elementType;
                }

                instance = Activator.CreateInstance(typeof(PlcArray), name, element, 0, 0) as PlcObject;
                if (instance != null)
                {
                    if (leafPlcObject != null)
                    {
                        if (instance is PlcArray plcArray)
                        {
                            plcArray.LeafElementType = leafPlcObject;
                        }
                    }

                    instance.ElemenType = elementType;
                    UpdateSize(pi, instance);
                }
            }
            else
            {
                if (plcType != null || _typeMatch.TryGetValue(arrayIndex == null ? pi.PropertyType : pi.PropertyType.GetElementType()!, out plcType))
                {
                    instance = Activator.CreateInstance(plcType, arrayIndex == null ? name : name + string.Format(CultureInfo.InvariantCulture, "[{0}]", arrayIndex)) as PlcObject;
                    UpdateSize(pi, instance);
                }
                else
                {
                    instance = Activator.CreateInstance(typeof(PlcStruct), arrayIndex == null ? name : name + string.Format(CultureInfo.InvariantCulture, "[{0}]", arrayIndex), pi.PropertyType) as PlcObject;
                }

                if (instance != null)
                {
                    instance.ElemenType = pi.PropertyType;
                }
            }


            if(instance != null && !string.IsNullOrWhiteSpace(originName))
            {
                instance.OriginName = originName;
            }
            UpdateRootAccessNotAllowed(instance);
            UpdateReadOnlyPoperty(pi, instance);
            UpdateSymbolicAccessName(pi, instance);
            return instance;
        }

        public static PlcObject? CreatePlcObjectForArrayIndex(PlcObject obj, int? arrayIndex, int from)
        {
            var plcType = obj.GetType();
            var plcObject = obj is PlcArray arrayElement ?
                Activator.CreateInstance(plcType, arrayIndex == null ? obj.Name : obj.Name + string.Format(CultureInfo.InvariantCulture, "[{0}]", arrayIndex), arrayElement.ArrayType, arrayElement.From, arrayElement.To) as PlcObject :
                Activator.CreateInstance(plcType, arrayIndex == null ? obj.Name : obj.Name + string.Format(CultureInfo.InvariantCulture, "[{0}]", arrayIndex)) as PlcObject;

            if (plcObject != null && arrayIndex != null)
            {
                switch (plcObject)
                {
                    case PlcBool plcbool:
                        {

                            plcbool.AssigneOffsetFrom((((int)arrayIndex - @from) * (obj.Size != null ? obj.Size.Bits : 0) + obj.Offset.Bits));

                        }
                        break;
                    case ISupportStringLengthAttribute plcString:
                        {
                            if (obj is ISupportStringLengthAttribute o)
                            {
                                plcString.AssigneLengthFrom(o);
                            }
                        }
                        break;
                }
            }
            return plcObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateReadOnlyPoperty(MemberInfo pi, PlcObject? plcObject)
        {
            if (plcObject != null)
            {
                var readOnlyAttribute = pi.GetCustomAttributes<ReadOnlyAttribute>().FirstOrDefault();
                if (readOnlyAttribute != null)
                {
                    plcObject.IsReadOnly = readOnlyAttribute.IsReadOnly;
                }
                var notAcessibleAttribute = pi.GetCustomAttributes<NotAccessibleAttribute>().FirstOrDefault();
                if (notAcessibleAttribute != null)
                {
                    // not accessible is also readonly
                    plcObject.IsNotAccessible = plcObject.IsReadOnly = notAcessibleAttribute.IsNotAccessible;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateSymbolicAccessName(MemberInfo pi, PlcObject? plcObject)
        {
            if (plcObject != null)
            {
                var aliasAttribute = pi.GetCustomAttributes<SymbolicAccessNameAttribute>().FirstOrDefault();
                if (aliasAttribute != null)
                {
                    plcObject.SymbolicAccessName = aliasAttribute.Name;
                }
                else
                {
                    plcObject.SymbolicAccessName = pi.Name;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateRootAccessNotAllowed(PlcObject? plcObject)
        {
            if (plcObject != null)
            {
                var attr = plcObject.ElemenType?.GetCustomAttributes<PlcTypeAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    plcObject.RootAccessNotAllowed = attr.RootAccessNotAllowed;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateSize(MemberInfo pi, PlcObject? plcObject, int dimension = 0)
        {
            if (plcObject is ISupportStringLengthAttribute s)
            {
                var stringLength = pi.GetCustomAttributes<StringLengthAttribute>().FirstOrDefault();
                if (stringLength != null)
                {
                    s.StringLength = stringLength.MaximumLength;
                }
            }
            else if (plcObject is PlcArray)
            {
                var bounds = pi.GetCustomAttributes<ArrayBoundsAttribute>().OfType<ArrayBoundsAttribute>().FirstOrDefault(x => x.Dimension == dimension);
                if (bounds != null && plcObject is PlcArray obj)
                {
                    obj.From = bounds.From;
                    obj.To = bounds.To;
                    obj.Dimension = bounds.Dimension;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (string name, string? originName) GetName(MemberInfo pi)
        {
            var aliasAttribute = pi.GetCustomAttributes<AliasNameAttribute>().FirstOrDefault();
            if (aliasAttribute != null)
            {
                return (aliasAttribute.Name, pi.Name);
            }
            return (pi.Name, null);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetOffsetFromAttribute(MemberInfo pi, ref int byteOffset, ref int bitOffset)
        {
            var mappingOffsets = pi.GetCustomAttributes<MappingOffsetAttribute>().FirstOrDefault();
            if (mappingOffsets != null)
            {
                var off = mappingOffsets.ByteOffset;
                if (byteOffset != off)
                {
                    byteOffset = off;
                    bitOffset = 0;
                }

                off = mappingOffsets.BitOffset;
                if (off != -1)
                {
                    bitOffset = off;
                }

                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type? GetTypeFromAttribute(MemberInfo pi)
        {
            var attribute = pi.GetCustomAttributes<PlcTypeAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                if (_typeNameMatch.TryGetValue(attribute.Name, out var plcType))
                {
                    return plcType;
                }
            }
            return null;
        }

        public static Type GetTypeForPlcObject(Type plcObjectType) => _reverseTypeMatch.TryGetValue(plcObjectType, out var retType) ? retType : typeof(object);
    }
}

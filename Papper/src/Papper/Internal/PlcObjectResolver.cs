using Papper.Attributes;
using Papper.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Papper.Internal
{
    internal static class PlcObjectResolver
    {

        public const string RootNodeName = "Plc";
        public const string InstancesNodeName = "Instances";
        public const string MetaDataNodeName = "MetaData";
        private static readonly Regex _regexIndexOfArrayPart = new("[\\[]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

        /// <summary>
        /// Syntax:
        /// Selector= Area[DbNumber]
        /// 
        /// IB: Input Area
        /// FB: Flag Area
        /// QB: Output Area
        /// TM: Timer Area
        /// CT: Counter Area
        /// DB: DataBlock Area
        /// 
        /// W = Word; X = Bool;  B = Byte; I = Int; S = String;
        /// 
        /// [TYPE][OFFSET][_CountOfData]
        /// 
        /// Bool
        /// [TYPE][OFFSET_Bit][_CountOfData]
        /// 
        /// String
        /// [TYPE][OFFSET_Stringlength][_CountOfData]
        /// </summary>
        /// <param name="plcObj"></param>
        /// <param name="plcObjects"></param>
        /// <param name="values"></param>
        internal static bool AddRawPlcObjects(ITreeNode plcObj, IDictionary<string, OperationItem> plcObjects, IEnumerable<string> values)
        {
            var adds = new Dictionary<string, OperationItem>();
            var updated = false;
            foreach (var value in values)
            {
                var dataType = new StringBuilder();
                foreach (var c in value.TakeWhile(char.IsLetter))
                {
                    dataType.Append(c);
                }

                var plcObject = DataTypeToPlcObject(value, dataType);

                if (plcObject != null)
                {
                    var parts = value.Substring(dataType.Length).Split(new[] { "_", ".", "," }, StringSplitOptions.RemoveEmptyEntries);
                    plcObject.Selector = plcObj.Name;
                    if (parts.Length >= 2)
                    {
                        if (plcObject is PlcBool)
                        {
                            plcObject.Offset.Bits = int.Parse(parts[1], CultureInfo.InvariantCulture);
                        }

                        if (plcObject is ISupportStringLengthAttribute plcs)
                        {
                            plcs.StringLength = int.Parse(parts[1], CultureInfo.InvariantCulture);
                        }

                        if (!(plcObject is PlcBool || plcObject is ISupportStringLengthAttribute) || parts.Length >= 3)
                        {
                            var length = int.Parse(parts.Last(), CultureInfo.InvariantCulture);
                            if (length > 0)
                            {
                                length--;
                            }

                            plcObject = new PlcArray(value, plcObject, 0, length);
                        }
                    }
                    plcObject.Offset.Bytes = int.Parse(parts[0], CultureInfo.InvariantCulture);
                    if (!plcObjects.ContainsKey(value))
                    {
                        adds.Add(value, new OperationItem(0, string.Empty, plcObject));
                    }
                }
                else
                {
                    ExceptionThrowHelper.ThrowInvalidVariableException($"{plcObj.Name}.{value}");
                }
            }

            if (adds.Any())
            {
                foreach (var item in adds)
                {
                    if (!plcObjects.ContainsKey(item.Key))
                    {
                        updated = true;
                        plcObjects.Add(item.Key, item.Value);
                    }
                }
            }
            return updated;
        }

        private static PlcObject? DataTypeToPlcObject(string value, StringBuilder dataType)
        {
            PlcObject? plcObject = null;
            switch (dataType.ToString())
            {
                case "X":
                case "BIT":
                    plcObject = new PlcBool(value);
                    break;
                case "B":
                case "BYTE":
                    plcObject = new PlcByte(value);
                    break;
                case "C":
                case "CHAR":
                    plcObject = new PlcChar(value);
                    break;
                case "DATE":
                    plcObject = new PlcDate(value);
                    break;
                case "DT":
                case "DATETIME":
                    plcObject = new PlcDateTime(value);
                    break;
                case "DI":
                case "DINT":
                    plcObject = new PlcDInt(value);
                    break;
                case "DW":
                case "DWORD":
                    plcObject = new PlcDWord(value);
                    break;
                case "I":
                case "INT":
                    plcObject = new PlcInt(value);
                    break;
                case "LDT":
                case "LDATETIME":
                    plcObject = new PlcLDateTime(value);
                    break;
                case "LI":
                case "LINT":
                    plcObject = new PlcLInt(value);
                    break;
                case "LT":
                case "LTIME":
                    plcObject = new PlcTime(value);
                    break;
                case "LW":
                case "LWORD":
                    plcObject = new PlcLWord(value);
                    break;
                case "R":
                case "REAL":
                    plcObject = new PlcReal(value);
                    break;
                case "LR":
                case "LREAL":
                    plcObject = new PlcLReal(value);
                    break;
                case "TIMEBCD":
                    plcObject = new PlcS5Time(value);
                    break;
                case "CT":
                case "COUNT":
                    plcObject = new PlcS7Counter(value);
                    break;
                case "SI":
                case "SINT":
                    plcObject = new PlcSInt(value);
                    break;
                case "S":
                case "STRING":
                    plcObject = new PlcString(value);
                    break;
                case "T":
                case "TIME":
                    plcObject = new PlcTime(value);
                    break;
                case "TOD":
                case "TIME_OF_DAY":
                    plcObject = new PlcTimeOfDay(value);
                    break;
                case "LTOD":
                case "LTIME_OF_DAY":
                    plcObject = new PlcLTimeOfDay(value);
                    break;
                case "UDI":
                case "UDINT":
                    plcObject = new PlcUDInt(value);
                    break;
                case "UI":
                case "UINT":
                    plcObject = new PlcUInt(value);
                    break;
                case "ULI":
                case "ULINT":
                    plcObject = new PlcULInt(value);
                    break;
                case "USI":
                case "USINT":
                    plcObject = new PlcUSInt(value);
                    break;
                case "WC":
                case "WCHAR":
                    plcObject = new PlcWChar(value);
                    break;
                case "W":
                case "WORD":
                    plcObject = new PlcWord(value);
                    break;
                case "WS":
                case "WSTRING":
                    plcObject = new PlcWString(value);
                    break;

            }

            return plcObject;
        }

        /// <summary>
        /// Adds the meta data of requested variables to the dictionary 
        /// </summary>
        internal static bool AddPlcObjects(ITreeNode plcObj, IDictionary<string, OperationItem> plcObjects, IEnumerable<string> values, string prefix = "", int offset = 0)
        {
            var updated = false;
            foreach (var value in values.Where(x => !plcObjects.ContainsKey(x)))
            {
                var baseOffset = offset;
                var symbolicPath = new StringBuilder();
                var item = value == "This" ? plcObj as PlcObject : plcObj.Get(new PlcMetaDataTreePath(value), ref baseOffset, ref symbolicPath) as PlcObject;
                if (item == null)
                {
                    ExceptionThrowHelper.ThrowInvalidVariableException($"{plcObj.Name}.{value}");
                    return false;
                }

                var key = prefix + value;

                if (!plcObjects.ContainsKey(key))
                {
                    try
                    {
                        plcObjects.Add(key, new OperationItem(baseOffset, symbolicPath.ToString(), item));
                    }
                    catch (Exception)
                    {
                        // This could throw an exception if another thread has already added the object.
                        // but we could ignore this, because this would be the same object
                    }
                    updated = true;
                }

                if (item is PlcStruct plcStruct)
                {
                    updated = AddPlcObjects(plcStruct, plcObjects, plcStruct.Childs.Select(child => child.Name), key + ".", baseOffset) || updated;
                }
            }
            return updated;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNotAccessibleElement(ITreeNode node, VariableListTypes accessMode)
        {
            if(node is PlcObject plco)
            {
                if(accessMode == VariableListTypes.Read)
                {
                    return plco.IsNotAccessible;
                }
                return plco.IsReadOnly;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRootNotAccessable(ITreeNode node)
        {
            if (node is PlcObject plco)
            {
                return plco.RootAccessNotAllowed;
            }
            return false;
        }

        /// <summary>
        /// Get all leafs from the meta tree, normally this should b only value types.
        /// </summary>
        /// <param name="obj">TreeNode</param>
        /// <param name="path">Path to node</param>
        /// <returns>List of leafs</returns>
        public static IEnumerable<string> GetLeafs(ITreeNode obj, ICollection<string> path, bool onlyWriteable = false)
        {
            var list = new List<string>();
            if (obj != null)
            {
                var match = _regexIndexOfArrayPart.Match(obj.Name);
                if (match.Success)
                {
                    var p = path.Last();
                    path = new List<string>(path.Take(path.Count - 1)) { p + obj.Name.Substring(match.Index) };
                }
                else
                {
                    path.Add(obj.Name);
                }


                var currentChilds = obj.Childs.Where(c => !onlyWriteable || !IsNotAccessibleElement(c, VariableListTypes.Write)).ToList();
                if (currentChilds.Any())
                {
                    if (obj is PlcArray)
                    {
                        foreach (var child in currentChilds)
                        {
                            var arrayChilds = child.Childs.Where(c => !onlyWriteable || !IsNotAccessibleElement(c, VariableListTypes.Write)).ToList();
                            if (arrayChilds.Any())
                            {
                                var nunmberOfChilds = arrayChilds.Count;
                                var internalPath = new List<string>(path.Take(path.Count - 1)) { obj.Name + child.Name };
                                foreach (var c in arrayChilds)
                                {
                                    list.AddRange(GetLeafs(c, internalPath.ToList(), onlyWriteable));
                                }
                            }
                            else
                            {
                                var internalPath = new List<string>(path.Take(path.Count - 1)) { child.Name };
                                list.Add(PlcMetaDataTreePath.CreateAbsolutePath(internalPath.Skip(1)).Path.Substring(1));
                            }
                        }
                    }
                    else
                    {
                        foreach (var child in currentChilds)
                        {
                            list.AddRange(GetLeafs(child, path.ToList(), onlyWriteable));
                        }
                    }

                }
                else
                {
                    list.Add(PlcMetaDataTreePath.CreateAbsolutePath(path.Skip(1)).Path.Substring(1));
                }
            }
            return list;
        }

        private static string GetAccessName(ITreeNode node)
        {
            if(node is PlcObject obj && !string.IsNullOrWhiteSpace(obj.SymbolicAccessName))
            {
                return obj.SymbolicAccessName!;
            }
            return node.Name;
        }



        private static readonly Regex _regex = new("\\[.*?\\]", RegexOptions.Compiled);

        public static IEnumerable<string> GetAccessibleBlocks(ITreeNode obj, ICollection<string> path, VariableListTypes accessMode, out bool hasNotAccessibleVariables, out List<string> notAccessible)
        {
            var list = new List<string>();
            notAccessible = new List<string>();
            hasNotAccessibleVariables = false;
            if (obj != null)
            {
                var currentName = GetAccessName(obj);
                var match = _regexIndexOfArrayPart.Match(currentName);
                if (match.Success)
                {
                    var p = path.Last();
                    path = new List<string>(path.Take(path.Count - 1)) { p + currentName.Substring(match.Index) };
                }
                else
                {
                    path.Add(currentName);
                }

                hasNotAccessibleVariables = IsRootNotAccessable(obj) || obj.Childs.Any(c => IsNotAccessibleElement(c, accessMode));
                var currentChilds = obj.Childs.Where(c => !IsNotAccessibleElement(c, accessMode)).ToList();
                if (hasNotAccessibleVariables || currentChilds.Any())
                {
                    if (!currentChilds.Any())
                    {
                        // nothing to add
                        notAccessible.Add((PlcMetaDataTreePath.CreateAbsolutePath(path).Path.Substring(1)));
                    }
                    else
                    {
                        if (obj is PlcArray)
                        {
                            foreach (var child in currentChilds)
                            {
                                var hasCurrentChildReadOnlyAttribute = child.Childs.Any(c => IsNotAccessibleElement(c, accessMode));
                                if (hasCurrentChildReadOnlyAttribute)
                                {
                                    hasNotAccessibleVariables = true;
                                }
                                var arrayChilds = child.Childs.Where(c => !IsNotAccessibleElement(c, accessMode)).ToList();

                                if (hasCurrentChildReadOnlyAttribute)
                                {
                                    var internalPath = new List<string>(path.Take(path.Count - 1)) { currentName + GetAccessName(child) };
                                    var notAccessibleChilds = child.Childs.Where(c => IsNotAccessibleElement(c, accessMode)).ToList();
                                    foreach (var c in notAccessibleChilds)
                                    {
                                        var internalElementPath = new List<string>(internalPath) { GetAccessName(c) };
                                        var internalAbsPath = PlcMetaDataTreePath.CreateAbsolutePath(internalElementPath).Path.Substring(1);
                                        string output = _regex.Replace(internalAbsPath, "[]");
                                        if (!notAccessible.Contains(output))
                                        {
                                            notAccessible.Add(output);
                                        }
                                    }
                                }

                                if (arrayChilds.Any())
                                {
                                    var numberOfChilds = arrayChilds.Count;
                                    var internalPath = new List<string>(path.Take(path.Count - 1)) { currentName + GetAccessName(child) };
                                    foreach (var c in arrayChilds)
                                    {
                                        var childVars = GetAccessibleBlocks(c, internalPath.ToList(), accessMode, out var hasNotAccessiblyChild, out var notAccessibleChild);
                                        if (!hasNotAccessiblyChild)
                                        {
                                            var internalElementPath = new List<string>(internalPath) { GetAccessName(c) };
                                            list.Add(PlcMetaDataTreePath.CreateAbsolutePath(internalElementPath.Skip(1)).Path.Substring(1));
                                        }
                                        else
                                        {
                                            hasNotAccessibleVariables = true;
                                            list.AddRange(childVars);
                                            foreach (var item in notAccessibleChild)
                                            {
                                                if (!notAccessible.Contains(item))
                                                {
                                                    notAccessible.Add(item);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // No Accessible Childs
                                    // var internalPath = new List<string>(path.Take(path.Count - 1)) { GetAccessName(child) };
                                    // list.Add(PlcMetaDataTreePath.CreateAbsolutePath(internalPath.Skip(1)).Path.Substring(1));
                                }
                            }
                            if (hasNotAccessibleVariables)
                            {
                                string output = _regex.Replace(PlcMetaDataTreePath.CreateAbsolutePath(path).Path.Substring(1), "[]");
                                if (!notAccessible.Contains(output))
                                {
                                    notAccessible.Add(output);
                                }
                            }
                        }
                        else
                        {
                            foreach (var child in currentChilds)
                            {
                                var childVars = GetAccessibleBlocks(child, path.ToList(), accessMode, out var hasNotAccessiblyChild, out var notAccessibleChild);
                                if (!hasNotAccessiblyChild)
                                {
                                    var internalPath = new List<string>(path) { GetAccessName(child) };
                                    list.Add(PlcMetaDataTreePath.CreateAbsolutePath(internalPath.Skip(1)).Path.Substring(1));
                                }
                                else
                                {
                                    hasNotAccessibleVariables = true;
                                    list.AddRange(childVars);
                                    foreach (var item in notAccessibleChild)
                                    {
                                        if (!notAccessible.Contains(item))
                                        {
                                            notAccessible.Add(item);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var child in obj.Childs.Where(c => IsNotAccessibleElement(c, accessMode)).ToList())
                        {
                            var internalPath = new List<string>(path) { GetAccessName(child) };
                            string output = _regex.Replace(PlcMetaDataTreePath.CreateAbsolutePath(internalPath).Path.Substring(1), "[]");
                            if (!notAccessible.Contains(output))
                            {
                                notAccessible.Add(output);
                            }
                        }
                    }

                }
                else
                {
                    list.Add(PlcMetaDataTreePath.CreateAbsolutePath(path.Skip(1)).Path.Substring(1));
                }
            }
            return list;
        }

        /// <summary>
        /// Try to get the mapping from MetaTree. If the data are not in the tree, try to create and add it. 
        /// </summary>
        internal static PlcObject? GetMapping(string? name, ITree tree, Type t, bool allowAddingWithoutMappingAttribute = false, MappingAttribute? useMapping = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var nodePathStack = new Stack<string>();
            nodePathStack.Push(RootNodeName);
            nodePathStack.Push(InstancesNodeName);
            foreach (var part in name!.Split('.'))
            {
                nodePathStack.Push(part);
            }

            var path = PlcMetaDataTreePath.CreateAbsolutePath(nodePathStack.Reverse());
            var offset = 0;
            var sb = new StringBuilder();
            if (!tree.TryGet(path, ref offset, ref sb, out var obj, true))
            {
                if (t == null)
                {
                    ExceptionThrowHelper.ThrowArgumentNullException(nameof(t));
                }

                var mapping = useMapping ?? t?.GetTypeInfo().GetCustomAttributes<MappingAttribute>().FirstOrDefault(m => m.Name == name);
                if (mapping?.Name == name)
                {
                    nodePathStack.Pop();
                    var plcObj = new PlcObjectRef(name, GetMetaData(tree, t!))
                    {
                        Offset = { Bytes = mapping.Offset },
                        Selector = mapping.Selector
                    };
                    PlcObject.AddPlcObjectToTree(plcObj, tree, PlcMetaDataTreePath.CreateAbsolutePath(nodePathStack.Reverse()));
                    obj = plcObj;
                }
                else if (allowAddingWithoutMappingAttribute)
                {
                    nodePathStack.Pop();
                    var plcObj = new PlcObjectRef(name, GetMetaData(tree, t!));
                    PlcObject.AddPlcObjectToTree(plcObj, tree, PlcMetaDataTreePath.CreateAbsolutePath(nodePathStack.Reverse()));
                    obj = plcObj;
                }
                else
                {
                    ExceptionThrowHelper.ThrowInvalidMappingNameException(name);
                }
            }
            return obj as PlcObject;
        }


        private static string NormalizeTypeName(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = new StringBuilder();
                foreach (var c in name.Where(c => c != '.'))
                {
                    result.Append(c);
                }
                return result.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Try to get Meta data from MetaTree. If the data are not in the tree, try to create and add it. 
        /// All the calculation of the offsets will be made in this method. the tree has two parts, one for MetaData and one for The Mappings.
        /// The mappings have only a Reference of the Meta data, so you shouldn't change the Offsets while using.
        /// </summary>
        private static PlcObject GetMetaData(ITree tree, Type t)
        {
            var nodePathStack = new Stack<string>();
            var name = NormalizeTypeName(t.FullName);
            nodePathStack.Push(RootNodeName);
            nodePathStack.Push(MetaDataNodeName);
            nodePathStack.Push(name);
            var path = PlcMetaDataTreePath.CreateAbsolutePath(nodePathStack.Reverse());
            var offset = 0;
            var sb = new StringBuilder();
            if (!tree.TryGet(path, ref offset, ref sb, out var obj))
            {
                var byteOffset = 0;
                var bitOffset = 0;
                nodePathStack.Pop();
                var parent = new PlcStruct(name, t);
                PlcObject.AddPlcObjectToTree(parent, tree, PlcMetaDataTreePath.CreateAbsolutePath(nodePathStack.Reverse()));
                nodePathStack.Push(parent.Name);
                PlcObject? pred = null;
                DebugOutPut("{0}{{", name);

                foreach (var pi in t.GetTypeInfo().DeclaredProperties.Where(x => x.GetCustomAttribute<IgnoreAttribute>()?.IsIgnored is not true))
                {
                    var plcObject = PlcObjectFactory.CreatePlcObject(pi);

                    if (plcObject is PlcArray plcObjectArray && (plcObjectArray.LeafElementType ?? plcObjectArray.ArrayType) is PlcStruct)
                    {
                        PlcArray current = plcObjectArray;
                        var reverseList = new List<PlcArray>() { current };
                        while (current?.ArrayType is PlcArray sub)
                        {
                            reverseList.Insert(0, sub);
                            current = sub;
                        }

                        if(current != null)
                        {
                            current.ArrayType = GetMetaData(tree, plcObjectArray.ElemenType!);

                            if (reverseList.Count > 1)
                            {
                                // recalculate sizes because we added an Element Type on the leave element
                                foreach (var item in reverseList.Skip(1))
                                {
                                    item.CalculateSize();
                                }
                            }
                        }


                    }
                    else if (plcObject is PlcStruct)
                    {
                        var refObject = new PlcObjectRef(plcObject.Name, GetMetaData(tree, pi.PropertyType));
                        if(refObject != null)
                        {
                            refObject.IsReadOnly = plcObject.IsReadOnly;
                            refObject.IsNotAccessible = plcObject.IsNotAccessible;
                            refObject.RootAccessNotAllowed = plcObject.RootAccessNotAllowed;
                            refObject.SymbolicAccessName = plcObject.SymbolicAccessName;
                            refObject.OriginName = plcObject.OriginName;
                        }

                        plcObject = refObject;
                    }

                    if (plcObject != null)
                    {
                        var hasCustomOffset = PlcObjectFactory.GetOffsetFromAttribute(pi, ref byteOffset, ref bitOffset);
                        AddPlcObject(tree, pred, plcObject, nodePathStack, ref byteOffset, ref bitOffset, hasCustomOffset);
                        pred = plcObject;
                        if(!parent.HasReadOnlyChilds && (plcObject.IsReadOnly || plcObject.HasReadOnlyChilds))
                        {
                            parent.HasReadOnlyChilds = true;
                        }
                        if (!parent.HasNotAccessibleChilds && (plcObject.IsNotAccessible || plcObject.HasNotAccessibleChilds))
                        {
                            parent.HasNotAccessibleChilds = true;
                            parent.HasReadOnlyChilds = true;
                        }
                        if(!parent.HasRootAccessNotAllowedChilds && (plcObject.RootAccessNotAllowed || plcObject.HasRootAccessNotAllowedChilds))
                        {
                            parent.HasRootAccessNotAllowedChilds = true;
                        }
                        
                    }
                }
                DebugOutPut("}} = {0}", parent.Size.Bytes);
                nodePathStack.Pop();
                
                obj = parent;
            }
            return (obj as PlcObject)!;
        }

        /// <summary>
        /// Calculates the Offset of the new Object, add it to the tree and update the offsets
        /// </summary>
        private static void AddPlcObject(ITree tree, PlcObject? pred, PlcObject plcObject, IEnumerable<string> nodePathStack, ref int byteOffset, ref int bitOffset, bool hasCustomOffset)
        {
            if (!hasCustomOffset)
            {
                CalculateOffset(pred, plcObject, ref byteOffset, ref bitOffset);
            }

            plcObject.Offset.Bytes = byteOffset;
            plcObject.Offset.Bits = bitOffset;
            DebugOutPut("{0}: Offset:{1,3}.{2}    Size={3}.{4}", plcObject.Name.PadRight(20), byteOffset, bitOffset, plcObject.Size == null ? 0 : plcObject.Size.Bytes, plcObject.Size == null ? 0 : plcObject.Size.Bits);
            PlcObject.AddPlcObjectToTree(plcObject, tree, PlcMetaDataTreePath.CreateAbsolutePath(nodePathStack.Reverse()));
            byteOffset += plcObject.Size == null ? 0 : plcObject.Size.Bytes;
            bitOffset += plcObject.Size == null ? 0 : plcObject.Size.Bits;
        }

        /// <summary>
        /// Calculate the real offsets in the PLC.
        /// </summary>
        internal static void CalculateOffset(PlcObject? pred, PlcObject? cur, ref int byteOffset, ref int bitOffset)
        {
            if (byteOffset % 2 == 0 && bitOffset == 0)
            {
                //No further checks needed
            }
            else
            {
                var prevIsBool = pred is PlcBool;
                var curIsBool = cur is PlcBool;
                var prevIsString = pred is PlcString;
                var prevIsByte = pred is PlcByte or PlcChar or PlcSInt or PlcUSInt;
                var curIsByte = cur is PlcByte or PlcChar or PlcSInt or PlcUSInt;
                var prevIsArray = pred is PlcArray;
                var curIsArray = cur is PlcArray;

                if (prevIsBool && (!curIsBool || bitOffset > 7) ||
                    prevIsArray)
                {
                    byteOffset += 1;
                    bitOffset = 0;
                }

                if (byteOffset % 2 != 0)
                {
                    if (curIsArray ||
                        !((prevIsByte && curIsByte) ||
                          (prevIsByte && curIsBool) ||
                          (prevIsBool && curIsByte) ||
                          (prevIsBool && curIsBool) ||
                          (prevIsString && curIsBool) ||
                          (prevIsString && curIsByte)))
                    {
                        byteOffset += 1;
                        bitOffset = 0;
                    }
                }
            }
        }


        [Conditional("DEBUG")]
        private static void DebugOutPut(string format, params object[] attributes) => Debug.WriteLine(format, attributes);
    }
}

using Papper.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace Papper.Types
{
    internal abstract class PlcObject : PlcMetaDataTreeNode, IPlcObject
    {
        public string? Selector { get; set; }
        public Type? ElemenType { get; set; }
        public bool AllowOddByteOffsetInArray { get; set; }

        public PlcSize Offset { get; } = new PlcSize();

        public virtual bool HasReadOnlyChilds { get; internal set; }
        public virtual bool HasNotAccessibleChilds { get; internal set; }
        public virtual bool HasRootAccessNotAllowedChilds { get; internal set; }
        public virtual bool IsReadOnly { get; internal set; }
        public virtual bool IsNotAccessible { get; internal set; }
        public virtual int? ArrayStartIndex { get; internal set; }
        public virtual bool RootAccessNotAllowed { get; internal set; }


        public virtual int ByteOffset => Offset.Bytes;
        public virtual int BitOffset => Offset.Bits;
        public virtual int ByteSize => Size == null ? 0 : Size.Bytes;
        public virtual int BitSize => Size == null ? 0 : Size.Bits;

        public abstract Type DotNetType { get; }

        public IEnumerable<IPlcObject> ChildVars => Childs.OfType<IPlcObject>();

        public virtual PlcSize? Size { get; protected set; }

        public static PlcObject? AddPlcObjectToTree(PlcObject obj, ITree tree, ITreePath path)
        {
            var node = PlcMetaDataTreePath.CreateNodePath(path, obj);
            if (node == null)
            {
                return null;
            }

            if (tree.Get(node) is not PlcObject metaDataNode)
            {
                lock (tree.Root)
                {
                    if (tree.Get(node) is PlcObject plcObj)
                    {
                        return plcObj;
                    }

                    tree.Root.AddChild(path, obj);
                }
                return obj;
            }
            return metaDataNode;
        }

        protected PlcObject(string name)
            : base(name)
        {
        }

        public override string ToString() => GetType().ToString();


        public abstract object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data);
        public abstract void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data);

        public virtual object? StringToObject(string value)
        {
            try
            {
                var type = PlcObjectFactory.GetTypeForPlcObject(GetType());
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch
            { }
            return null;
        }

        public virtual bool AreDataEqual(object obj1, object obj2)
        {
            var t1 = obj1.GetType();
            var t2 = obj2.GetType();

            if (t1 == t2) { return t1 != typeof(ExpandoObject) ? ElementEqual(obj1, obj2) : DynamicObjectCompare(obj1, obj2); }
            try { return ElementEqual(obj1, Convert.ChangeType(obj2, t1, CultureInfo.InvariantCulture)); } catch { }
            return false;
        }

        private bool ElementEqual(object obj1, object obj2)
        {
            if (obj1 is IEnumerable list1 && obj2 is IEnumerable list2)
            {
                var enumerator1 = list1.GetEnumerator();
                var enumerator2 = list2.GetEnumerator();
                while (true)
                {
                    var e1 = enumerator1.MoveNext();
                    var e2 = enumerator2.MoveNext();
                    if (e1 && e2)
                    {
                        if (!AreDataEqual(enumerator1.Current, enumerator2.Current))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return e1 == e2; //Length not the same?
                    }
                }
            }
            return obj1.Equals(obj2);
        }

        private bool DynamicObjectCompare(object obj1, object obj2)
        {

            if (obj1 is IDictionary<string, object> dictionary1 && obj2 is IDictionary<string, object> dictionary2)
            {
                foreach (var o1 in dictionary1)
                {
                    if (!dictionary2.TryGetValue(o1.Key, out var o2) || !AreDataEqual(o1.Value, o2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}

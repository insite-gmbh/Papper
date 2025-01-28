using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Papper.Types
{
    internal class PlcObjectRef : PlcObject
    {
        internal const string _refMarker = "\\";
        private readonly PlcObject _referencedObject;

        public override Type DotNetType => _referencedObject.DotNetType;

        public override PlcSize? Size => _referencedObject?.Size;

        public override int ByteOffset => Offset.Bytes;
        public override int BitOffset => Offset.Bits;
        public override int ByteSize => Size == null ? 0 : Size.Bytes;
        public override int BitSize => Size == null ? 0 : Size.Bits;

        public override bool HasReadOnlyChilds => _referencedObject != null ? _referencedObject.HasReadOnlyChilds : false;
        public override bool HasNotAccessibleChilds => _referencedObject != null ? _referencedObject.HasNotAccessibleChilds : false;

        public override bool HasRootAccessNotAllowedChilds => _referencedObject != null ? _referencedObject.HasRootAccessNotAllowedChilds : false;

        public PlcObjectRef(string name, PlcObject reference) :
            base(name) => _referencedObject = reference;

        public override object ConvertFromRaw(PlcObjectBinding plcObjectBinding, Span<byte> data)
         => _referencedObject.ConvertFromRaw(plcObjectBinding, data);

        public override void ConvertToRaw(object? value, PlcObjectBinding plcObjectBinding, Span<byte> data)
         => _referencedObject.ConvertToRaw(value, plcObjectBinding, data);

        public override IEnumerable<ITreeNode> Childs => _referencedObject.Childs;

        public override void AddChild(ITreeNode child) => ExceptionThrowHelper.ThrowNotSupportedException();

        public override ITreeNode RemoveChild(string name)
        {
            ExceptionThrowHelper.ThrowNotSupportedException();
            return default!;
        }

        public override void AddChild(ITreePath path, ITreeNode node) => ExceptionThrowHelper.ThrowNotSupportedException();

        public override ITreeNode? GetChildByName(string name) => _referencedObject.GetChildByName(name);

        public override ITreeNode? Get(ITreePath path, ref int offset, ref StringBuilder symbolicPath, bool getRef = false)
        {
            offset += Offset.Bytes;
            if (path.IsPathToCurrent && getRef)
            {
                return this;
            }

            return _referencedObject.Get(path, ref offset, ref symbolicPath);
        }

        public override void Accept(VisitNode visit) => _referencedObject.Accept(visit);

        public override void ReverseAccept(VisitNode visit) => _referencedObject.ReverseAccept(visit);

        public override void ClearCache() => _referencedObject.ClearCache();

    }
}

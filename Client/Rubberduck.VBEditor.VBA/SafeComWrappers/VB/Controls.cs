using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using System.Collections;
using System.Collections.Generic;
using VB = Microsoft.Vbe.Interop;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public sealed class Controls : SafeComWrapper<VB.Forms.Controls>, IControls
    {
        public Controls(VB.Forms.Controls? target, bool rewrapping = false) 
            : base(target!, rewrapping)
        {
        }

        public int Count => IsWrappingNullReference ? 0 : Target.Count;

        public IControl this[object index] => IsWrappingNullReference ? new Control(null!) : new Control((VB.Forms.Control) Target.Item(index));

        IEnumerator<IControl> IEnumerable<IControl>.GetEnumerator()
        {
            // soft-casting because ImageClass doesn't implement IControl
            return new ComWrapperEnumerator<IControl>(Target, comObject => new Control((comObject as VB.Forms.Control)!));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return IsWrappingNullReference
                ? new List<IEnumerable>().GetEnumerator()
                : ((IEnumerable<IControl>) this).GetEnumerator();
        }

        public override bool Equals(ISafeComWrapper<VB.Forms.Controls> other)
        {
            return IsEqualIfNull(other) || (other != null && ReferenceEquals(other.Target, Target));
        }

        public bool Equals(IControls? other)
        {
            return Equals((other as SafeComWrapper<VB.Forms.Controls>)!);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : Target.GetHashCode();
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
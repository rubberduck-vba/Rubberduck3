using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using System;
using VB = Microsoft.Vbe.Interop.VB6;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VB6
{
    public class Application : SafeComWrapper<VB.Application>, IApplication
    {
        public Application(VB.Application target, bool rewrapping = false)
            :base(target, rewrapping)
        {
        }

        public string Version => IsWrappingNullReference ? null : Target.Version;

        public override bool Equals(ISafeComWrapper<VB.Application> other)
        {
            return IsEqualIfNull(other) || (other != null && other.Target.Version == Version);
        }

        public bool Equals(IApplication other)
        {
            return Equals(other as SafeComWrapper<VB.Application>);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : HashCode.Combine(Target);
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
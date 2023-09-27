// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;

namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public class SolidWorksApp : HostApplicationBase<Interop.SldWorks.Extensibility.Application>
    {
        public SolidWorksApp(IVBE vbe) : base(vbe, "SolidWorks") { }
    }
}

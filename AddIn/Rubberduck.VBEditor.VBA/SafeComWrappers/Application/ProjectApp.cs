// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;

namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public class ProjectApp : HostApplicationBase<Microsoft.Office.Interop.MSProject.Application>
    {
        public ProjectApp(IVBE vbe) : base(vbe, "MSProject", true) { }
    }
}

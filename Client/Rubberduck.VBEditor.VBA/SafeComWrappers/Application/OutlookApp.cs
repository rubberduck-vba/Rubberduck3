using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    [ComVisible(false)]
    public class OutlookApp : HostApplicationBase<Microsoft.Office.Interop.Outlook.Application>
    {
        public OutlookApp(IVBE vbe) : base(vbe, "Outlook", true) { }
    }
}

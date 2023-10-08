// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
using Microsoft.Extensions.Logging;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;

namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public class PublisherApp : HostApplicationBase<Microsoft.Office.Interop.Publisher.Application>
    {
        public PublisherApp(IVBE vbe) : base(vbe, "Publisher", true) { }
    }
}

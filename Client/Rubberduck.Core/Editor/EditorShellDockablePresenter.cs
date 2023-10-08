using Microsoft.Extensions.Logging;
using Rubberduck.UI.WinForms;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.VBEditor.UI;

namespace Rubberduck.Core.Editor
{

    public class EditorShellDockablePresenter : DockableToolwindowPresenter
    {
        public EditorShellDockablePresenter(IVBE vbe, IAddIn addin, IEditorShellWindowProvider viewFactory, ILogger<EditorShellDockablePresenter> logger) 
            : base(vbe, addin, viewFactory.Create(), logger)
        {
        }
    }
}
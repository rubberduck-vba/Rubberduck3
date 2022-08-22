using Rubberduck.UI;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.Core.Editor
{
    public class EditorShellDockablePresenter : DockableToolwindowPresenter
    {
        public EditorShellDockablePresenter(IVBE vbe, IAddIn addin, EditorShellWindow view) 
            : base(vbe, addin, view)
        {
        }
    }
}

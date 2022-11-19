using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rubberduck.Core.Editor
{
    public class EditorTabViewModel : ViewModelBase, IEditorTabViewModel
    {
        public string Title { get; set; }
        public string ModuleContent { get; set; }
        public IEditorSettings EditorSettings { get; set; }

        public string CurrentStatus { get; set; }

        public void UpdateStatus(string status)
        {
            throw new System.NotImplementedException();
        }
    }

    public class EditorShellViewModel : ViewModelBase, IEditorShellViewModel
    {
        public EditorShellViewModel()
        {
            var module = new EditorTabViewModel
            {
                Title = "Module1",
                ModuleContent = "Option Explicit\n",
            };
            Documents.Add(module);
        }

        public ObservableCollection<IEditorTabViewModel> Documents { get; } = new ObservableCollection<IEditorTabViewModel>();

        public string CurrentStatus { get; set; } = "Ready";

        public void UpdateStatus(string status)
        {
            throw new System.NotImplementedException();
        }
    }

    public class EditorShellDockablePresenter : DockableToolwindowPresenter
    {
        public EditorShellDockablePresenter(IVBE vbe, IAddIn addin, EditorShellWindow view) 
            : base(vbe, addin, view)
        {
        }
    }
}
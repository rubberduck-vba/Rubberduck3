using Rubberduck.Settings;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rubberduck.Core.Editor
{
    public class MemberInfoViewModel
    {
        public string IconSource { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DocString { get; set; }
        public int Offset { get; set; }

        public void Test() { }
    }

    public class EditorTabViewModel : ViewModelBase, IEditorTabViewModel
    {
        public string Title { get; set; }
        public string ModuleContent { get; set; }
        public IEditorSettings EditorSettings { get; set; }

        public IEnumerable<MemberInfoViewModel> MemberList { get; set; } = new MemberInfoViewModel[] 
        {
            new MemberInfoViewModel
            {
                DisplayName = "(declarations)",
                Offset = 1,
            },
            new MemberInfoViewModel
            {
                Name = "DoSomething",
                DisplayName = "DoSomething()",
                DocString = "Does something, surely",
                IconSource = "MethodIcon"
            }
        };

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
                EditorSettings = new EditorSettings() // TODO inject from actual settings
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
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
        private static readonly ImageSource MethodIcon = new BitmapImage(new Uri("pack://application:,,,/Rubberduck.Resources;component/Icons/Custom/PNG/ObjectMethod.png"));

        public ImageSource IconSource { get; set; } = MethodIcon;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DocString { get; set; }
        public int Offset { get; set; }

        public void Test() { }
    }

    public class EditorTabViewModel : ViewModelBase, IEditorTabViewModel
    {
        private string _currentStatus;

        public EditorShellViewModel Shell { get; set; }

        public string Title { get; set; }
        public string ModuleContent { get; set; }
        public IEditorSettings EditorSettings { get; set; }

        public IEnumerable<MemberInfoViewModel> MemberList { get; set; } = new MemberInfoViewModel[]
        {
            new MemberInfoViewModel
            {
                DisplayName = "(declarations)",
                Offset = 1,
                IconSource = null
            },
            new MemberInfoViewModel
            {
                Name = "DoSomething",
                DisplayName = "DoSomething()",
                DocString = "Does something, surely",
            }
        };

        public string CurrentStatus 
        { 
            get => _currentStatus;
            set
            {
                if (_currentStatus != value)
                {
                    _currentStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateStatus(string status)
        {
            Shell.UpdateStatus(status);
        }
    }

    public class EditorShellViewModel : ViewModelBase, IEditorShellViewModel
    {
        public EditorShellViewModel()
        {
            var module = new EditorTabViewModel
            {
                Shell = this,
                Title = "Module1",
                ModuleContent = "Option Explicit\n",
                EditorSettings = new EditorSettings() // TODO inject from actual settings
            };
            Documents.Add(module);
        }

        public ObservableCollection<IEditorTabViewModel> Documents { get; } = new ObservableCollection<IEditorTabViewModel>();

        private string _currentStatus;
        public string CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (_currentStatus != value)
                {
                    _currentStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateStatus(string status)
        {
            CurrentStatus = status;
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
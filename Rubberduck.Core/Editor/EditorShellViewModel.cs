using Rubberduck.Settings;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System.Collections.ObjectModel;

namespace Rubberduck.Core.Editor
{
    public class EditorShellViewModel : ViewModelBase, IEditorShellViewModel
    {
        public EditorShellViewModel()
        {
            var module = new CodePaneViewModel
            {
                Shell = this,
                Title = "Module1",
                Content = "Option Explicit\n",
                EditorSettings = new EditorSettings() // TODO inject from actual settings
            };
            Documents.Add(module);
        }

        public ObservableCollection<ICodePaneViewModel> Documents { get; } = new ObservableCollection<ICodePaneViewModel>();

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
}
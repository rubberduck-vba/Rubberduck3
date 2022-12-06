using Rubberduck.Parsing.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System.Windows.Input;

namespace Rubberduck.Core.Editor.Tools
{
    public class SyncPanelModuleViewModel : ViewModelBase, ISyncPanelModuleViewModel
    {
        public SyncPanelModuleViewModel()
        {
        }

        public ICommand LoadCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SyncCommand { get; }


        private QualifiedModuleName _qualifiedModuleName;
        public QualifiedModuleName QualifiedModuleName
        {
            get => _qualifiedModuleName;
            set
            {
                if (_qualifiedModuleName != value)
                {
                    _qualifiedModuleName = value;
                    OnPropertyChanged();
                }
            }
        }

        private ModuleType _moduleType;
        public ModuleType ModuleType
        {
            get => _moduleType;
            set
            {
                if (_moduleType != value)
                {
                    _moduleType = value;
                    OnPropertyChanged();
                }
            }
        }

        private ModuleSyncState _state;
        public ModuleSyncState State
        {
            get => _state;  
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

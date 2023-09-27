using Rubberduck.InternalApi.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.Unmanaged.Model.Abstract;
using System.Windows.Input;

namespace Rubberduck.Core.Editor.Tools
{
    public class SyncPanelModuleViewModel : ViewModelBase, ISyncPanelModuleViewModel
    {
        public SyncPanelModuleViewModel(ILoadCommand loadCommand, IOpenCommand openCommand, ISyncCommand syncCommand)
        {
            LoadCommand = loadCommand;
            OpenCommand = openCommand;
            SyncCommand = syncCommand;
        }

        public ICommand LoadCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SyncCommand { get; }


        private IQualifiedModuleName _qualifiedModuleName;
        public IQualifiedModuleName QualifiedModuleName
        {
            get => _qualifiedModuleName;
            set
            {
                if (!_qualifiedModuleName.Equals(value))
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

using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Services.Abstract
{
    public abstract class ToolWindowViewModelBase : ViewModelBase, IToolWindowViewModel
    {
        protected ToolWindowViewModelBase(DockingLocation location,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
        {
            DockingLocation = location;
            ShowSettingsCommand = showSettingsCommand;
            CloseToolWindowCommand = closeToolWindowCommand;

            Header = Title;
        }

        private DockingLocation _location;
        public DockingLocation DockingLocation
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ShowSettingsCommand { get; }
        public string ShowSettingsCommandParameter => SettingKey;

        public ICommand CloseToolWindowCommand { get; }

        public abstract string SettingKey { get; }
        public abstract string Title { get; }

        private object _header;
        public virtual object Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _content;
        public object Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set
            {
                if (_isPinned != value)
                {
                    _isPinned = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual bool ShowPinButton { get; } = true;
        public virtual bool ShowGearButton { get; } = true;
        public virtual bool ShowCloseButton { get; } = true;

        public string AcceptButtonText { get; set; }
        public string CancelButtonText { get; set; }
    }
}

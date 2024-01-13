using Rubberduck.UI.Command.SharedHandlers;
using System.Windows.Input;
using Resx = Rubberduck.Resources.v3.RubberduckMessages;

namespace Rubberduck.UI.Windows
{
    public abstract class WindowViewModel : ViewModelBase
    {
        protected WindowViewModel(
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
        {
            ShowSettingsCommand = showSettingsCommand;
            CloseToolWindowCommand = closeToolWindowCommand;
        }

        private bool _showPinButton;
        public bool ShowPinButton
        {
            get => _showPinButton;
            set
            {
                if (_showPinButton != value)
                {
                    _showPinButton = value;
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

        public string Title { get; set; } = "Title";
        public string SettingKey { get; set; }

        public bool ShowGearButton => ShowSettingsCommand != null;
        public ICommand? ShowSettingsCommand { get; init; }

        public bool ShowCloseButton => CloseToolWindowCommand != null;
        public ICommand? CloseToolWindowCommand { get; }
        public string? ShowSettingsCommandParameter => SettingKey;

        public virtual bool ShowAcceptButton { get; } = true;
        public virtual bool ShowCancelButton { get; } = true;

        public virtual string AcceptButtonText { get; } = Resx.MessageActionButton_Accept;
        public virtual string CancelButtonText { get; } = Resx.MessageActionButton_Cancel;
    }
}

using Rubberduck.UI.Command;
using System.Windows.Input;

namespace Rubberduck.UI
{
    public abstract class DialogWindowViewModel : ViewModelBase, IDialogWindowViewModel
    {
        public static MessageAction[] ActionCloseOnly { get; } = new[] { MessageAction.CloseAction };
        public static MessageAction[] ActionAcceptCancel { get; } = new[] { MessageAction.AcceptAction, MessageAction.CancelAction };

        protected DialogWindowViewModel(string title, MessageActionCommand[] actions, ICommand? showSettingsCommand = null)
        {
            Title = title;
            Actions = actions;
            IsEnabled = true;
            ShowSettingsCommand = showSettingsCommand;
        }

        protected abstract void ResetToDefaults();

        public ICommand? ShowSettingsCommand { get; init; }

        public MessageActionCommand[] Actions { get; init; }

        public MessageAction? SelectedAction { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                    ResetToDefaults();
                }
            }
        }

        public string Title { get; init; }
    }
}

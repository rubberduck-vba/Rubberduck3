﻿using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shared.Message;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Windows
{
    public abstract class DialogWindowViewModel : ViewModelBase, IDialogWindowViewModel
    {
        public static MessageAction[] ActionCloseOnly { get; } = new[] { MessageAction.CloseAction };
        public static MessageAction[] ActionAcceptCancel { get; } = new[] { MessageAction.AcceptAction, MessageAction.CancelAction };

        private readonly UIServiceHelper _service;
        protected DialogWindowViewModel(UIServiceHelper service, string title, MessageActionCommand[] actions, ICommand? showSettingsCommand = null, object? showSettingsCommandParameter = null)
        {
            _service = service;

            Title = title;
            Actions = actions;
            IsEnabled = true;
            ShowSettingsCommand = showSettingsCommand;
            ShowSettingsCommandParameter = showSettingsCommandParameter;
        }

        public bool ExtendWindowChrome => _service.Settings.EditorSettings.ExtendWindowChrome;
        protected abstract void ResetToDefaults();

        public ICommand? ShowSettingsCommand { get; init; }
        public object? ShowSettingsCommandParameter { get; init; }

        public bool ShowGearButton => ShowSettingsCommand != null && ShowSettingsCommandParameter != null;

        public MessageActionCommand[] Actions { get; init; }
        public bool ShowAcceptButton => Actions.Any(action => action.MessageAction == MessageAction.AcceptAction || action.MessageAction == MessageAction.CloseAction);
        public string AcceptButtonText => Actions.Single(action => action.MessageAction == MessageAction.AcceptAction || action.MessageAction == MessageAction.CloseAction).MessageAction.Text;

        public bool ShowCancelButton => Actions.Any(action => action.MessageAction == MessageAction.CancelAction);
        public string CancelButtonText => Actions.SingleOrDefault(action => action.MessageAction == MessageAction.CancelAction)?.MessageAction.Text ?? MessageAction.CancelAction.Text;

        public MessageAction? SelectedAction { get; set; }
        public bool IsCancelled => SelectedAction is null || !SelectedAction.IsDefaultAction;

        private bool _isEnabled = true;
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

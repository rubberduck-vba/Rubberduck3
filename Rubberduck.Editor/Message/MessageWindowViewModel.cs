using Microsoft.Extensions.Logging;
using Rubberduck.UI;
using System;
using System.Linq;

namespace Rubberduck.Editor.Message
{
    public class MessageWindowViewModel : ViewModelBase, IMessageWindowViewModel
    {
        public static MessageAction[] ActionCloseOnly { get; } = new[] { MessageAction.CloseAction };
        public static MessageAction[] ActionAcceptCancel { get; } = new[] { MessageAction.AcceptAction, MessageAction.CancelAction };

        public MessageWindowViewModel(MessageModel model)
        {
            Key = model.Key;
            Message = model.Message;
            Verbose = model.Verbose;
            Title = model.Title;
            Level = model.Level;
        }

        public string Key { get; init; }
        public string Message { get; init; }
        public string? Verbose { get; init; }
        public string Title { get; init; }
        public LogLevel Level { get; init; }

        private MessageAction[] _actions = ActionCloseOnly;
        public MessageAction[] Actions
        {
            get => _actions;
            init
            {
                if (_actions != value)
                {
                    _actions = value;
                    OnPropertyChanged();
                    if (_actions?.Length > 0)
                    {
                        SelectedAction = _actions.First(e => e.IsDefaultAction);
                    }
                    else
                    {
                        throw new InvalidOperationException("At least one MessageAction item is required.");
                    }
                }
            }
        }

        private MessageAction? _action;

        public MessageAction? SelectedAction
        {
            get => _action;
            set
            {
                if (_action != value)
                {
                    _action = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _doNotShowAgain;
        public bool DoNotShowAgain
        {
            get => _doNotShowAgain;
            set
            {
                if (_doNotShowAgain != value)
                {
                    _doNotShowAgain = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

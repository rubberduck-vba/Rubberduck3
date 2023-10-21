using Microsoft.Extensions.Logging;
using Rubberduck.Editor.Command;
using Rubberduck.Editor.FileMenu;
using Rubberduck.UI;
using System;
using System.Linq;

namespace Rubberduck.Editor.Message
{
    public class MessageWindowViewModel : ViewModelBase, IMessageWindowViewModel
    {
        /// <summary>
        /// Parameterless constructor for designer view.
        /// </summary>
        public MessageWindowViewModel()
        {
            _actions = new[]{new AcceptMessageActionCommand(null!, null!, MessageAction.CloseAction)};
        }

        public MessageWindowViewModel(MessageModel model, MessageActionsProvider actions)
        {
            Key = model.Key;
            Message = model.Message;
            Verbose = model.Verbose;
            Title = model.Title;
            Level = model.Level;

            _actions = actions.Close();
        }

        public string Key { get; init; } = "DT-Message";
        public string Message { get; init; } = "Message goes here";
        public string? Verbose { get; init; } = null;
        public string Title { get; init; } = "Title";
        public LogLevel Level { get; init; } = LogLevel.Information;

        private MessageActionCommand[] _actions;
        public MessageActionCommand[] Actions
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
                        SelectedAction = _actions.First(e => e.MessageAction.IsDefaultAction).MessageAction;
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

        private bool _isEnabled = true;
        /// <remarks>
        /// Use an inverted-bool converter to use with e.g. "do not show again" checkboxes.
        /// </remarks>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

﻿using Microsoft.Extensions.Logging;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Windows;
using System;
using System.Linq;

namespace Rubberduck.UI.Shared.Message
{
    public class MessageWindowViewModel : WindowViewModel, IMessageWindowViewModel
    {
        /// <summary>
        /// Parameterless constructor for designer view.
        /// </summary>
        public MessageWindowViewModel()
            : base(null!, null!)
        {
            _actions = new[] { new AcceptMessageActionCommand(null!, MessageAction.CloseAction) };
        }

        public MessageWindowViewModel(MessageModel model, MessageActionCommand[] actions,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
            : base(showSettingsCommand, closeToolWindowCommand)
        {
            SettingKey = model.Key;
            Message = model.Message;
            Verbose = model.Verbose;
            Title = model.Title;
            Level = model.Level;

            _actions = actions;
        }

        public string Message { get; init; } = "Message goes here";
        public string? Verbose { get; init; } = null;
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
                        SelectedAction = _actions.Single(e => e.MessageAction.IsDefaultAction).MessageAction;
                    }
                    else
                    {
                        throw new InvalidOperationException("At least one MessageAction item is required.");
                    }
                }
            }
        }

        public override bool ShowAcceptButton => Actions.Any(action => action.MessageAction.IsDefaultAction);
        public override string AcceptButtonText => Actions.SingleOrDefault(action => action.MessageAction.IsDefaultAction)?.MessageAction.Text ?? base.AcceptButtonText;

        public override bool ShowCancelButton => Actions.Any(action => !action.MessageAction.IsDefaultAction);
        public override string CancelButtonText => Actions.FirstOrDefault(action => !action.MessageAction.IsDefaultAction)?.MessageAction.Text ?? base.CancelButtonText;

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

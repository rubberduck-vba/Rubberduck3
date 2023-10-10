using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Xaml.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Message
{
    public interface IMessageService
    {
        void ShowMessage(string key, string message, string title, string? verbose = default);
        void ShowWarning(string key, string message, string title, string? verbose = default);
        void ShowError(string key, string message, string title, string? verbose = default);

        MessageAction ConfirmMessage(string key, string message, string title, string? verbose = default);
        MessageAction ConfirmWarning(string key, string message, string title, string? verbose = default);
        MessageAction ConfirmError(string key, string message, string title, string? verbose = default);

        MessageAction Show(LogLevel level, string key, string message, string title, string? verbose = default, params MessageAction[] actions);
    }

    public class MessageService : IMessageService
    {
        private readonly ISettingsProvider<RubberduckSettings> _settings;
        private readonly ILogger _logger;

        public MessageService(ISettingsProvider<RubberduckSettings> settings, ILogger<MessageService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public MessageAction ConfirmError(string key, string message, string title, string? verbose = null)
            => Show(LogLevel.Error, key, message, title, verbose, MessageAction.AcceptAction, MessageAction.CancelAction);

        public MessageAction ConfirmMessage(string key, string message, string title, string? verbose = null)
            => Show(LogLevel.Information, key, message, title, verbose, MessageAction.AcceptAction, MessageAction.CancelAction);

        public MessageAction ConfirmWarning(string key, string message, string title, string? verbose = null)
            => Show(LogLevel.Warning, key, message, title, verbose, MessageAction.AcceptAction, MessageAction.CancelAction);

        public MessageAction Show(LogLevel level, string key, string message, string title, string? verbose = null, params MessageAction[] actions)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();
            var model = new MessageWindowViewModel
            {
                Title = title,
                Key = key,
                Message = message,
                Verbose = verbose,
                DoNotShowAgain = false,
                Actions = actions,
                SelectedAction = actions?.FirstOrDefault(e => e.IsDefaultAction)
            };
            if (CanShowMessageKey(key))
            {
                var view = new MessageWindow(model);
                view.ShowDialog();

                _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {model.SelectedAction.ResourceKey}");
                return model.SelectedAction;
            }
            else
            {
                _logger.LogTrace(trace, "Message key was blocked by the user; message will not be shown.", $"Key: {key} ({level})");
                return model.SelectedAction;
            }
        }

        public void ShowError(string key, string message, string title, string? verbose = null)
            => Show(LogLevel.Error, key, message, title, verbose, MessageAction.CloseAction);

        public void ShowMessage(string key, string message, string title, string? verbose = null)
            => Show(LogLevel.Information, key, message, title, verbose, MessageAction.CloseAction);

        public void ShowWarning(string key, string message, string title, string? verbose = null)
            => Show(LogLevel.Warning, key, message, title, verbose, MessageAction.CloseAction);

        private bool CanShowMessageKey(string key) => !_settings.Settings.SkippedMessageKeys.Contains(key);
    }

    public class MessageWindowViewModel : ViewModelBase, IMessageWindowViewModel
    {
        public string Key { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Verbose { get; set; }

        private MessageAction[] _actions;
        public MessageAction[] Actions 
        { 
            get => _actions;
            set
            {
                if (_actions != value )
                {
                    _actions = value;
                    OnPropertyChanged();
                    if (_actions?.Length > 0)
                    {
                        SelectedAction = _actions.FirstOrDefault(e => e.IsDefaultAction) ?? _actions.First();
                    }
                    else
                    {
                        throw new InvalidOperationException("At least one MessageAction item is required.");
                    }
                }
            }
        }

        private MessageAction _action;
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

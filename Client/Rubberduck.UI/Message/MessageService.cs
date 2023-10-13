using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Xaml.Message;
using System;
using System.Linq;

namespace Rubberduck.UI.Message
{
    public class MessageActionResult
    {
        public static MessageActionResult Undefined { get; } = new MessageActionResult
        {
            IsEnabled = true,
            MessageAction = MessageAction.Undefined,
        };

        /// <summary>
        /// Represents the action (button) selected by the user.
        /// </summary>
        public MessageAction MessageAction { get; init; } = MessageAction.Undefined;

        /// <summary>
        /// <c>false</c> if the user has checked a <em>do not show this message again</em> checkbox.
        /// </summary>
        public bool IsEnabled { get; init; } = true;
    }

    public interface IMessageService
    {
        MessageActionResult ShowMessageRequest(MessageRequestModel model);
        MessageActionResult ShowMessage(MessageModel model);
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

        public MessageActionResult ShowMessageRequest(MessageRequestModel model)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();
            var viewModel = new MessageWindowViewModel(model);

            if (CanShowMessageKey(viewModel.Key))
            {
                var view = new MessageWindow(viewModel);
                view.ShowDialog();

                var selection = viewModel.SelectedAction;
                if (selection is not null)
                {
                    _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {selection.Value.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection.Value, IsEnabled = viewModel.DoNotShowAgain };
                }

                _logger.LogWarning(trace, "User has closed the message window, but no message action was set. This is likely a bug.");
            }
            else
            {
                _logger.LogTrace(trace, "Message key was disabled by the user; message will not be shown.", $"Key: {viewModel.Key} ({viewModel.Level})");
            }

            return new MessageActionResult { MessageAction = MessageAction.Undefined, IsEnabled = viewModel.DoNotShowAgain };
        }

        public MessageActionResult ShowMessage(MessageModel model)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();
            var viewModel = new MessageWindowViewModel(model);

            if (CanShowMessageKey(viewModel.Key))
            {
                var view = new MessageWindow(viewModel);
                view.ShowDialog();

                var selection = viewModel.SelectedAction;
                if (selection is not null)
                {
                    _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {selection.Value.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection.Value, IsEnabled = viewModel.DoNotShowAgain };
                }

                _logger.LogWarning(trace, "User has closed the message window, but no message action was set. This is likely a bug.");
            }
            else
            {
                _logger.LogTrace(trace, "Message key was disabled by the user; message will not be shown.", $"Key: {viewModel.Key} ({viewModel.Level})");
            }

            return new MessageActionResult { MessageAction = MessageAction.Undefined, IsEnabled = viewModel.DoNotShowAgain };
        }

        private bool CanShowMessageKey(string key) => _settings.Settings.LanguageClientSettings.DisabledMessageKeys.Contains(key);
    }

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
                if (_actions != value )
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

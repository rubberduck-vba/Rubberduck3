using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Linq;

namespace Rubberduck.Editor.Message
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
                    _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {selection.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection, IsEnabled = viewModel.DoNotShowAgain };
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
                    _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {selection.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection, IsEnabled = viewModel.DoNotShowAgain };
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
}

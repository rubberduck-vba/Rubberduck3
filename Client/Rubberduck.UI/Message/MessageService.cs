﻿using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using System;
using System.Linq;

namespace Rubberduck.UI.Message
{
    public interface IMessageService
    {
        /// <summary>
        /// Displays a message to the user, requesting an action.
        /// </summary>
        /// <returns>
        /// <c>MessageActionResult.Disabled</c> if the model key is disabled.
        /// </returns>
        MessageActionResult ShowMessageRequest(MessageRequestModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null);

        /// <summary>
        /// Displays a message to the user.
        /// </summary>
        /// <returns>
        /// <c>MessageActionResult.Disabled</c> if the model key is disabled.
        /// </returns>
        MessageActionResult ShowMessage(MessageModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null);

        /// <summary>
        /// Displays user-facing exception error message, including the stack trace.
        /// </summary>
        /// <remarks>
        /// If the specified key does not exist in <c>RubberduckUI</c> resource strings, the <c>LogLevel</c> is used as a title.
        /// </remarks>
        /// <param name="key">The resource key for the message. Used for tracking whether or not this message should be shown.</param>
        /// <param name="exception">The exception to display details about.</param>
        /// <param name="level">Specify a 'level' that the implementation may use to display a corresponding icon.</param>
        void ShowError(string key, Exception exception, LogLevel level = LogLevel.Error);
    }

    public class MessageService : IMessageService
    {
        private readonly ISettingsProvider<RubberduckSettings> _settings;
        private readonly ILogger _logger;
        private readonly IMessageWindowFactory _viewFactory;

        private readonly MessageActionsProvider _actionsProvider;

        public MessageService(ISettingsProvider<RubberduckSettings> settings, ILogger<MessageService> logger,
            IMessageWindowFactory viewFactory,
            MessageActionsProvider actionsProvider)
        {
            _settings = settings;
            _logger = logger;
            _viewFactory = viewFactory;
            _actionsProvider = actionsProvider;
        }

        public MessageActionResult ShowMessageRequest(MessageRequestModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

            if (CanShowMessageKey(model.Key))
            {
                var (view, viewModel) = _viewFactory.Create(model, actions ?? (provider => _actionsProvider.OkCancel()));
                view.ShowDialog();

                var selection = viewModel.SelectedAction;
                if (selection is not null)
                {
                    _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {selection.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection, IsEnabled = viewModel.IsEnabled };
                }

                _logger.LogWarning(trace, "User has closed the message window, but no message action was set. This is likely a bug.");
                return viewModel.IsEnabled ? MessageActionResult.Default : MessageActionResult.Disabled;
            }
            else
            {
                _logger.LogTrace(trace, "Message key was disabled by the user; message will not be shown.", $"Key: {model.Key}  ({model.Level})");
            }

            return MessageActionResult.Disabled;
        }

        public MessageActionResult ShowMessage(MessageModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

            if (CanShowMessageKey(model.Key))
            {
                var (view, viewModel) = _viewFactory.Create(model, actions);
                view.ShowDialog();

                var selection = viewModel.SelectedAction;
                if (selection is not null)
                {
                    _logger.LogTrace(trace, "User has closed the message window.", $"Selected action: {selection.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection, IsEnabled = viewModel.IsEnabled };
                }

                _logger.LogWarning(trace, "User has closed the message window, but no message action was set. This is likely a bug.");
                return viewModel.IsEnabled ? MessageActionResult.Default : MessageActionResult.Disabled;
            }
            else
            {
                _logger.LogTrace(trace, "Message key was disabled by the user; message will not be shown.", $"Key: {model.Key} ({model.Level})");
            }

            return MessageActionResult.Disabled;
        }

        public void ShowError(string key, Exception exception, LogLevel level = LogLevel.Error)
        {
            var model = new MessageModel
            {
                Key = key,
                Level = level,
                Message = exception.Message,
                Verbose = exception.ToString(), // TODO make a markdown formatter for exception details
                Title = RubberduckUI.ResourceManager.GetString(key) ?? level.ToString()
            };
            ShowMessage(model, provider => provider.OkOnly());
        }

        private bool CanShowMessageKey(string key) => _settings.Settings.GeneralSettings.DisabledMessageKeys.Contains(key);
    }
}

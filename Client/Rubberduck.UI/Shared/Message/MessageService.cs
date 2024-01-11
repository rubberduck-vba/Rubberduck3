﻿using Microsoft.Extensions.Logging;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.Unmanaged.UIContext;
using System;
using System.Linq;

namespace Rubberduck.UI.Shared.Message
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

    public class MessageService : UIServiceHelper, IMessageService
    {
        private readonly IUiDispatcher _ui;
        private readonly IMessageWindowFactory _viewFactory;

        public MessageService(RubberduckSettingsProvider settings, ILogger<MessageService> logger,
            IUiDispatcher ui,
            IMessageWindowFactory viewFactory,
            PerformanceRecordAggregator performance)
            : base(logger, settings, performance)
        {
            _ui = ui;
            _viewFactory = viewFactory;
        }

        protected override void OnUserFacingException(Exception exception, string? message)
        {
            ShowError(exception.TargetSite?.Name ?? exception.GetType().Name, exception);
        }

        public MessageActionResult ShowMessageRequest(MessageRequestModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null)
        {
            if (CanShowMessageKey(model.Key))
            {
                static MessageActionCommand[] defaultActions(MessageActionsProvider provider) => provider.OkCancel();
                MessageAction? selection = null;
                IMessageWindowViewModel? viewModel = null;

                _ui.Invoke(() =>
                {
                    var (view, viewModel) = _viewFactory.Create(model, actions ?? defaultActions);
                    view.ShowDialog();

                    selection = viewModel.SelectedAction;
                });

                viewModel = viewModel ?? throw new InvalidOperationException();

                if (selection is not null)
                {
                    LogTrace("User has closed the message window.", $"Selected action: {selection.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection, IsEnabled = viewModel.IsEnabled };
                }

                LogWarning("User has closed the message window, but no message action was set. This is likely a bug.");
                return viewModel.IsEnabled ? MessageActionResult.Default : MessageActionResult.Disabled;
            }
            else
            {
                LogTrace("Message key was disabled by the user; message will not be shown.", $"Key: {model.Key} ({model.Level})");
            }

            return MessageActionResult.Disabled;
        }

        public MessageActionResult ShowMessage(MessageModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null)
        {
            if (CanShowMessageKey(model.Key))
            {
                MessageAction? selection = null;
                IMessageWindowViewModel? viewModel = null;

                _ui.Invoke(() =>
                {
                    var (view, viewModel) = _viewFactory.Create(model, actions);
                    view.ShowDialog();

                    selection = viewModel.SelectedAction;
                });

                viewModel = viewModel ?? throw new InvalidOperationException();

                if (selection is not null)
                {
                    LogTrace("User has closed the message window.", $"Selected action: {selection.ResourceKey}");
                    return new MessageActionResult { MessageAction = selection, IsEnabled = viewModel.IsEnabled };
                }

                LogWarning("User has closed the message window, but no message action was set. This is likely a bug.");
                return viewModel.IsEnabled ? MessageActionResult.Default : MessageActionResult.Disabled;
            }
            else
            {
                LogTrace("Message key was disabled by the user; message will not be shown.", $"Key: {model.Key} ({model.Level})");
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

        private bool CanShowMessageKey(string key) => !Settings.GeneralSettings.DisabledMessageKeys.Contains(key);
    }
}

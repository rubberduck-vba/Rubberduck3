using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Linq;

namespace Rubberduck.Editor.Message
{
    public interface IMessageWindowFactory
    {
        (MessageWindow view, IMessageWindowViewModel viewModel) Create<TModel>(TModel model) where TModel : MessageModel;
    }

    public class MessageWindowFactory : IMessageWindowFactory
    {
        /// <summary>
        /// Parameterless constructor for designer view.
        /// </summary>
        public MessageWindowFactory() { }

        public (MessageWindow view, IMessageWindowViewModel viewModel) Create<TModel>(TModel model) where TModel : MessageModel
        {
            var viewModel = new MessageWindowViewModel(model);
            var view = new MessageWindow(viewModel);
            return (view, viewModel);
        }
    }

    public record class MessageActionResult
    {
        public static MessageActionResult Default { get; } = new MessageActionResult
        {
            IsEnabled = true,
            MessageAction = MessageAction.Undefined,
        };

        public static MessageActionResult Disabled { get; } = new MessageActionResult
        {
            IsEnabled = false,
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
        /// <summary>
        /// Displays a message to the user, requesting an action.
        /// </summary>
        /// <returns>
        /// <c>MessageActionResult.Disabled</c> if the model key is disabled.
        /// </returns>
        MessageActionResult ShowMessageRequest(MessageRequestModel model);

        /// <summary>
        /// Displays a message to the user.
        /// </summary>
        /// <returns>
        /// <c>MessageActionResult.Disabled</c> if the model key is disabled.
        /// </returns>
        MessageActionResult ShowMessage(MessageModel model);
    }

    public class MessageService : IMessageService
    {
        private readonly ISettingsProvider<RubberduckSettings> _settings;
        private readonly ILogger _logger;
        private readonly IMessageWindowFactory _viewFactory;

        public MessageService(ISettingsProvider<RubberduckSettings> settings, ILogger<MessageService> logger,
            IMessageWindowFactory viewFactory)
        {
            _settings = settings;
            _logger = logger;
            _viewFactory = viewFactory;
        }

        public MessageActionResult ShowMessageRequest(MessageRequestModel model)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

            if (CanShowMessageKey(model.Key))
            {
                var (view, viewModel) = _viewFactory.Create(model);
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

        public MessageActionResult ShowMessage(MessageModel model)
        {
            var trace = _settings.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

            if (CanShowMessageKey(model.Key))
            {
                var (view, viewModel) = _viewFactory.Create(model);
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

        private bool CanShowMessageKey(string key) => _settings.Settings.LanguageClientSettings.DisabledMessageKeys.Contains(key);
    }
}

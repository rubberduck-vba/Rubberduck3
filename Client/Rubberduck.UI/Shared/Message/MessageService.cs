using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using System;
using System.Linq;

namespace Rubberduck.UI.Shared.Message;

public class MessageService : UIServiceHelper, IMessageService
{
    private readonly IMessageWindowFactory _viewFactory;

    public MessageService(RubberduckSettingsProvider settings, ILogger<MessageService> logger,
        IMessageWindowFactory viewFactory,
        PerformanceRecordAggregator performance)
        : base(logger, settings, performance)
    {
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

            RunOnMainThread(() =>
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
            MessageWindow? view = null;
            IMessageWindowViewModel? viewModel = null;

            RunOnMainThread(() =>
            {
                (view, viewModel) = _viewFactory.Create(model, actions);
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

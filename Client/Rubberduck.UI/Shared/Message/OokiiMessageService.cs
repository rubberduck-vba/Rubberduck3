using Microsoft.Extensions.Logging;
using Ookii.Dialogs.Wpf;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.General;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.Resources;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Messages = Rubberduck.Resources.v3.RubberduckMessages;
namespace Rubberduck.UI.Shared.Message;

public class OokiiMessageService : UIServiceHelper, IMessageService
{
    private readonly MessageActionsProvider _provider;


    private static readonly Dictionary<LogLevel, TaskDialogIcon> _dialogIconPerLogLevel = new Dictionary<LogLevel, TaskDialogIcon>()
    {
        [LogLevel.None] = TaskDialogIcon.Custom,
        [LogLevel.Trace] = TaskDialogIcon.Custom,
        [LogLevel.Debug] = TaskDialogIcon.Information,
        [LogLevel.Information] = TaskDialogIcon.Information,
        [LogLevel.Warning] = TaskDialogIcon.Warning,
        [LogLevel.Error] = TaskDialogIcon.Error,
        [LogLevel.Critical] = TaskDialogIcon.Error,
    };

    private static readonly Icon _rubberduckIcon = Resources.v3.Icons.ducky;
    private static readonly Icon _rubberduckLogo = Resources.v3.Icons.black_vector_ducky;

    public OokiiMessageService(MessageActionsProvider provider,
        ILogger<UIServiceHelper> logger, 
        RubberduckSettingsProvider settingsProvider, 
        PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _provider = provider;
    }

    private bool IsMessageKeyDisabled(string key) => Settings.GeneralSettings.DisabledMessageKeys.Contains(key);

    public void ShowError(string key, Exception exception, LogLevel level = LogLevel.Error)
    {
        if (IsMessageKeyDisabled(key))
        {
            LogInformation($"Message key '{key}' is disabled; error message was not shown.", exception.Message);
            return;
        }

        var dialog = new TaskDialog()
        {
            AllowDialogCancellation = true,
            Content = Messages.ResourceManager.GetString(key) ?? $"[missing key:{key}]",
            ExpandedInformation = exception.ToString(),
            ButtonStyle = TaskDialogButtonStyle.Standard,
            MinimizeBox = false,
            ExpandedByDefault = false,
            WindowTitle = RubberduckUI.Rubberduck,
            WindowIcon = _rubberduckIcon,
            CustomMainIcon = _rubberduckLogo,
            MainIcon = _dialogIconPerLogLevel[level],

            //FooterIcon = TaskDialogIcon.Information,
            //Footer = $"https://rd3.rubberduckvba.com/errors/{key}",
            //EnableHyperlinks = true,

            VerificationText = Messages.ResourceManager.GetString(nameof(Messages.DoNotShowAgain)),
            IsVerificationChecked = false,
        };

        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));

        Logger.LogInformation("Showing error message key {key}", key);
        dialog.ShowDialog();

        if (dialog.IsVerificationChecked)
        {
            DisableMessageKey(key);
        }
    }

    private void DisableMessageKey(string key)
    {
        // that's... annoying.
        var updatedMessageKeys = Settings.GeneralSettings.GetSetting<DisabledMessageKeysSetting>()!.WithDisabledMessageKeys(key);
        var updatedGeneralSettings = (GeneralSettings)Settings.GeneralSettings.WithSetting(updatedMessageKeys);
        var updated = (RubberduckSettings)Settings.WithSetting(updatedGeneralSettings);
        SettingsProvider.Write(updated);
    }

    public MessageActionResult ShowMessage(MessageModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null)
    {
        if (IsMessageKeyDisabled(model.Key))
        {
            LogInformation($"Message key '{model.Key}' is disabled; message was not shown.");
            return MessageActionResult.Disabled;
        }

        var dialog = new TaskDialog()
        {
            AllowDialogCancellation = true,
            Content = model.Message,
            ExpandedInformation = model.Verbose,
            ButtonStyle = TaskDialogButtonStyle.Standard,
            MinimizeBox = false,
            ExpandedByDefault = false,
            WindowTitle = model.Title,
            WindowIcon = _rubberduckIcon,
            CustomMainIcon = _rubberduckLogo,
            MainIcon = _dialogIconPerLogLevel[model.Level],

            //FooterIcon = TaskDialogIcon.Information,
            //Footer = $"https://rd3.rubberduckvba.com/messages/{model.Key}",
            //EnableHyperlinks = true,

            VerificationText = Messages.ResourceManager.GetString(nameof(Messages.DoNotShowAgain)),
            IsVerificationChecked = false,
        };

        var okButton = new TaskDialogButton(ButtonType.Ok);
        var resultMap = new Dictionary<TaskDialogButton, MessageActionResult>()
        {
            [okButton] = new MessageActionResult { MessageAction = MessageAction.AcceptAction },
        };

        if (actions is null)
        {
            dialog.Buttons.Add(okButton);
        }
        else
        {
            var buttons = actions.Invoke(_provider).Select(e =>
                (Info: e, Button: new TaskDialogButton(e.MessageAction.Text)
                {
                    Default = e.MessageAction.IsDefaultAction,
                    ButtonType = ButtonType.Custom,
                    Text = e.MessageAction.Text,
                }));

            foreach (var button in buttons)
            {
                dialog.Buttons.Add(button.Button);
                resultMap.Add(button.Button, new MessageActionResult
                {
                    MessageAction = button.Info.MessageAction
                });
            }
        }

        Logger.LogInformation("Showing message key {key}", model.Key);
        var result = dialog.ShowDialog();
        if (result != null)
        {
            if (dialog.IsVerificationChecked)
            {
                DisableMessageKey(model.Key);
            }
            return resultMap[result] with { IsEnabled = !dialog.IsVerificationChecked };
        }

        return new MessageActionResult
        {
            MessageAction = MessageAction.CancelAction,
            IsEnabled = !dialog.IsVerificationChecked
        };
    }

    public MessageActionResult ShowMessageRequest(MessageRequestModel model, Func<MessageActionsProvider, MessageActionCommand[]>? actions = null)
    {
        if (IsMessageKeyDisabled(model.Key))
        {
            LogInformation($"Message key '{model.Key}' is disabled; message was not shown.");
            return MessageActionResult.Disabled;
        }

        var dialog = new TaskDialog()
        {
            AllowDialogCancellation = true,
            Content = model.Message,
            ExpandedInformation = model.Verbose,
            ButtonStyle = TaskDialogButtonStyle.Standard,
            MinimizeBox = false,
            ExpandedByDefault = false,
            WindowTitle = model.Title,
            WindowIcon = _rubberduckIcon,
            CustomMainIcon = _rubberduckLogo,
            MainIcon = _dialogIconPerLogLevel[model.Level],

            //FooterIcon = TaskDialogIcon.Information,
            //Footer = $"https://rd3.rubberduckvba.com/messages/{model.Key}",
            //EnableHyperlinks = true,

            VerificationText = Messages.ResourceManager.GetString(nameof(Messages.DoNotShowAgain)),
            IsVerificationChecked = false,
        };

        var okButton = new TaskDialogButton(ButtonType.Ok);
        var resultMap = new Dictionary<TaskDialogButton, MessageActionResult>()
        {
            [okButton] = new MessageActionResult { MessageAction = MessageAction.AcceptAction },
        };

        if (model.MessageActions is null || model.MessageActions.Length == 0)
        {
            dialog.Buttons.Add(okButton);
        }
        else
        {
            var buttons = model.MessageActions.Select(e =>
                (Info: e, Button: new TaskDialogButton(e.Text)
                {
                    Default = e.IsDefaultAction,
                    ButtonType = ButtonType.Custom,
                    Text = e.Text,
                })).OrderBy(e => !e.Button.Default);

            foreach (var button in buttons)
            {
                dialog.Buttons.Add(button.Button);
                resultMap.Add(button.Button, new MessageActionResult { MessageAction = button.Info });
            }
        }

        Logger.LogInformation("Showing message prompt key {key}", model.Key);
        var result = dialog.ShowDialog();
        if (result != null)
        {
            if (dialog.IsVerificationChecked)
            {
                DisableMessageKey(model.Key);
            }
            return resultMap[result] with { IsEnabled = !dialog.IsVerificationChecked };
        }

        return new MessageActionResult
        {
            MessageAction = MessageAction.CancelAction,
            IsEnabled = !dialog.IsVerificationChecked
        };
    }
}

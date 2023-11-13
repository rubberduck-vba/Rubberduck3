using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.UI;
using Rubberduck.UI.Message;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class ShowMessageRequestHandler : ShowMessageRequestHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IMessageService _messages;

        public ShowMessageRequestHandler(ServerPlatformServiceHelper service, IMessageService messages)
        {
            _service = service;
            _messages = messages;
        }

        private static readonly IDictionary<MessageType, LogLevel> _typeMap = new Dictionary<MessageType, LogLevel>
        {
            [MessageType.Log] = LogLevel.None,
            [MessageType.Info] = LogLevel.Information,
            [MessageType.Warning] = LogLevel.Warning,
            [MessageType.Error] = LogLevel.Error
        };

        public override async Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = MessageActionResult.Default;
            if (_typeMap.TryGetValue(request.Type, out var level))
            {
                _service.TryRunAction(() =>
                {
                    var actions = request.Actions?.Select(e => e.ToMessageAction()).ToArray();
                    if (actions is null)
                    {
                        _service.LogWarning("ShowMessageRequestParams did not include any message actions; using default Accept/Cancel actions. This is likely a bug.");
                        actions = new[] { MessageAction.AcceptAction, MessageAction.CancelAction };
                    }

                    var model = MessageRequestModel.For(level, request.Message, actions);
                    var generalSettings = _service.Settings.GeneralSettings;
                    if (!result.IsEnabled)
                    {
                        // type casts for clarity.. TODO come up with a better API for updating settings.
                        var disabledKeysSetting = generalSettings.GetSetting<DisabledMessageKeysSetting>();
                        var updatedDisabledKeys = disabledKeysSetting.WithDisabledMessageKeys(model.Key);
                        var updatedGeneralSettings = (GeneralSettings)generalSettings.WithSetting(updatedDisabledKeys);
                        var updatedSettings = (RubberduckSettings)_service.Settings.WithSetting(updatedGeneralSettings);
                        _service.SettingsService.Write(updatedSettings);
                    }
                }, nameof(ShowMessageRequestHandler));
            }
            else
            {
                _service.LogWarning("ShowMessageRequestParams was for an unmapped message type.", $"Message: '{request.Message}'");
            }

            return await Task.FromResult(result.ToMessageActionItem(request));
        }
    }
}

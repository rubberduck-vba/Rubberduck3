using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.ServerPlatform;
using Rubberduck.UI.Shared.Message;
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
                    if (result.MessageAction.IsDefaultAction && !result.IsEnabled)
                    {
                        DisabledMessageKeysSetting.DisableMessageKey(model.Key, _service.SettingsProvider);
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

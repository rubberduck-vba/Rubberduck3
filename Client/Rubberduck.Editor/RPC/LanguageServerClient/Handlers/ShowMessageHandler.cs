using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.UI.Shared.Message;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class ShowMessageHandler : ShowMessageHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IMessageService _messages;

        public ShowMessageHandler(ServerPlatformServiceHelper service, IMessageService messages)
        {
            _service = service;
            _messages = messages;
        }

        private static readonly IDictionary<MessageType, LogLevel> _typeMap = new Dictionary<MessageType, LogLevel>
        {
            [MessageType.Info] = LogLevel.Information,
            [MessageType.Warning] = LogLevel.Warning,
            [MessageType.Error] = LogLevel.Error
        };

        public override async Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_typeMap.TryGetValue(request.Type, out var level))
            {
                _service.TryRunAction(() =>
                {
                    var model = MessageModel.FromShowMessageParams(request.Message, level);
                    var result = _messages.ShowMessage(model);
                    if (result != MessageActionResult.Disabled)
                    {
                        var generalSettings = _service.Settings.GeneralSettings;
                        if (result.MessageAction.IsDefaultAction)
                        {
                            DisabledMessageKeysSetting.DisableMessageKey(model.Key, _service.SettingsProvider);
                        }
                    }
                    else
                    {
                        _service.LogTrace("Key is disabled, message was not shown.", $"Key: '{model.Key}'");
                    }
                }, nameof(ShowMessageHandler));
            }
            else
            {
                // MessageType.Log does not pop a message box, but should appear in a server trace toolwindow.
                _service.LogInformation(request.Message);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}

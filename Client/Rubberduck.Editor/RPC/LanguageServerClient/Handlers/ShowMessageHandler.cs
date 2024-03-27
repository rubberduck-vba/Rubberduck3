using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.ServerPlatform;
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
                    _messages.ShowMessage(MessageModel.FromShowMessageParams(request.Message, level));
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

using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Rubberduck.ServerPlatform;
using System;
using System.Collections.Generic;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class LogMessageHandler : LogMessageHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;

        public LogMessageHandler(ServerPlatformServiceHelper service)
        {
            _service = service;
        }

        private static readonly IDictionary<MessageType, Action<ServerPlatformServiceHelper, string>> MessageTypeMap = 
            new Dictionary<MessageType, Action<ServerPlatformServiceHelper, string>>
            {
                [MessageType.Error] = (service, message) => service.LogError(message),
                [MessageType.Warning] = (service, message) => service.LogWarning(message),
                [MessageType.Info] = (service, message) => service.LogInformation(message),
                [MessageType.Log] = (service, message) => service.LogTrace(message),
            };

        public override async Task<Unit> Handle(LogMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var service = _service;
            var message = request.Message;
            var type = request.Type;

            service.RunAction(() =>
            {
                if (MessageTypeMap.TryGetValue(type, out var action))
                {
                    action.Invoke(service, message);
                }
                else
                {
                    service.LogDebug(message);
                }
            }, nameof(LogMessageHandler));
            
            return await Task.FromResult(Unit.Value);
        }
    }
}

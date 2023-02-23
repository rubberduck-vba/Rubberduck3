using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Disconnect
    internal class DisconnectHandler : JsonRpcRequestHandler<DisconnectRequest, DisconnectResult>
    {
        private readonly ServerState _serverState;

        public DisconnectHandler(ILogger logger, ServerState serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<DisconnectResult> HandleAsync(DisconnectRequest request)
        {
            var client = request.Params.ToObject<ClientInfo>();

            if (_serverState.Disconnect(client.Name, out var disconnected))
            {
                Logger.LogInformation($"Client {disconnected} has disconnected from this server.");
                return await Task.FromResult(new DisconnectResult());
            }

            throw new ApplicationException("The operation failed.");
        }
    }
}

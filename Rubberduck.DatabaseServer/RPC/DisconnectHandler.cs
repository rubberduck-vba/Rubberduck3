using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.ServerPlatform;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;
using Rubberduck.ServerPlatform.Services;

namespace Rubberduck.DatabaseServer.RPC
{
    [Method(JsonRpcMethods.DatabaseServer.Disconnect, Direction.ClientToServer)]
    internal class DisconnectHandler : JsonRpcRequestHandler<DisconnectRequest, DisconnectResult>
    {
        private readonly IServerStateService _serverState;

        public DisconnectHandler(ILogger<DisconnectHandler> logger, IServerStateService serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<DisconnectResult> HandleAsync(DisconnectRequest request)
        {
            var client = request.ClientInfo;

            if (_serverState.RemoveClient(client))
            {
                Logger.LogInformation($"Client '{client.Name}' has disconnected from this server.");
                return await Task.FromResult(DisconnectResult.Success);
            }
            
            throw new OperationNotCompletedException(GetType().Name, "The operation failed.", $"Client '{client.Name}' was not removed from server state.");
        }
    }
}

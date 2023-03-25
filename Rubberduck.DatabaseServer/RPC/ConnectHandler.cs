using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.ServerPlatform;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;
using Rubberduck.ServerPlatform.Services;

namespace Rubberduck.DatabaseServer.RPC
{
    internal class ConnectHandler : JsonRpcRequestHandler<ConnectRequest, ConnectResult>
    {
        private readonly IServerStateService _serverState;

        public ConnectHandler(ILogger<ConnectHandler> logger, IServerStateService serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        [Method(JsonRpcMethods.DatabaseServer.Connect, Direction.ClientToServer)]
        public override Task<ConnectResult> Handle(ConnectRequest request, CancellationToken cancellationToken)
            => base.Handle(request, cancellationToken);

        protected override async Task<ConnectResult> HandleAsync(ConnectRequest request)
        {
            if (_serverState.AddClient(request.ClientInfo))
            {
                return await Task.FromResult(ConnectResult.Success);
            }

            throw new OperationNotCompletedException(GetType().Name, verbose: $"Client '{request.ClientInfo.Name}' (v{request.ClientInfo.Version} PID:{request.ClientInfo.ProcessId}) was not connected.");
        }
    }
}
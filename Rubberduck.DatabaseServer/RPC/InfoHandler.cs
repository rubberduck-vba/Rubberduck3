using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;
using Rubberduck.ServerPlatform.Services;

namespace Rubberduck.DatabaseServer.RPC
{
    [Method(JsonRpcMethods.DatabaseServer.Info, Direction.ClientToServer)]
    internal class InfoHandler : JsonRpcRequestHandler<InfoRequest, InfoResult>
    {
        private readonly IServerStateService _serverState;

        public InfoHandler(ILogger<InfoHandler> logger, IServerStateService serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<InfoResult> HandleAsync(InfoRequest request)
        {
            var info = _serverState.GetServerProcessInfo();
            return await Task.FromResult(new InfoResult { ServerInfo = info });
        }
    }
}

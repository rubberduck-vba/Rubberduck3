using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform.Platform;
using Rubberduck.ServerPlatform.Platform.Model;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.Database.RPC.Connect
{
    internal class ConnectHandler : JsonRpcRequestHandler<ConnectRequest, SuccessResult>
    {
        private readonly ServerState _serverState;

        public ConnectHandler(ILogger logger, ServerState serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<SuccessResult> HandleAsync(ConnectRequest request)
        {
            var client = request.Params.ToObject<RpcClientInfo>();
            if (_serverState.Connect(client))
            {
                return await Task.FromResult(new SuccessResult());
            }

            throw new ApplicationException("The operation failed.");
        }
    }
}

using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Connect
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

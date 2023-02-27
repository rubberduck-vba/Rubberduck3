using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Disconnect
{ 
    internal class DisconnectHandler : JsonRpcRequestHandler<DisconnectRequest, SuccessResult>
    {
        private readonly ServerState _serverState;

        public DisconnectHandler(ILogger logger, ServerState serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<SuccessResult> HandleAsync(DisconnectRequest request)
        {
            var client = request.Params.ToObject<RpcClientInfo>();

            if (_serverState.Disconnect(client.Name, out var disconnected))
            {
                Logger.LogInformation($"Client {disconnected} has disconnected from this server.");
                return await Task.FromResult(new SuccessResult());
            }

            throw new ApplicationException("The operation failed.");
        }
    }
}

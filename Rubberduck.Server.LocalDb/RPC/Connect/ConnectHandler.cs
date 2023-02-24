using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Connect
{
    internal class ConnectHandler : JsonRpcRequestHandler<ConnectRequest, ConnectResult>
    {
        private readonly ServerState _serverState;

        public ConnectHandler(ILogger logger, ServerState serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override async Task<ConnectResult> HandleAsync(ConnectRequest request)
        {
            var client = request.Params.ToObject<ClientInfo>();
            if (_serverState.Connect(client))
            {
                return await Task.FromResult(new ConnectResult());
            }

            throw new ApplicationException("The operation failed.");
        }
    }
}

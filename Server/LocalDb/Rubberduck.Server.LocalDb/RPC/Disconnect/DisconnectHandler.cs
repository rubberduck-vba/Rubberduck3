using OmniSharp.Extensions.JsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Disconnect
{
    internal class DisconnectHandler : IJsonRpcRequestHandler<DisconnectRequest, DisconnectResult>
    {
        public DisconnectHandler()
        {

        }

        public Task<DisconnectResult> Handle(DisconnectRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

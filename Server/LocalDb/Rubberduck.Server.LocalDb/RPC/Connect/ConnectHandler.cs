using OmniSharp.Extensions.JsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Connect
{
    internal class ConnectHandler : IJsonRpcRequestHandler<ConnectRequest, ConnectResult>
    {
        public ConnectHandler()
        {

        }

        public Task<ConnectResult> Handle(ConnectRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

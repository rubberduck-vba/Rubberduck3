using OmniSharp.Extensions.JsonRpc;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Info
{
    public class InfoHandler : IJsonRpcRequestHandler<InfoRequest, InfoResult>
    {
        public Task<InfoResult> Handle(InfoRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

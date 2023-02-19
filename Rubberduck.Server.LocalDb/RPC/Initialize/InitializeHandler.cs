using OmniSharp.Extensions.JsonRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Initialize
{
    internal class InitializeHandler : IJsonRpcRequestHandler<InitializeRequest, InitializeResult>
    {
        public InitializeHandler()
        {

        }

        public Task<InitializeResult> Handle(InitializeRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

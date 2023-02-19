using MediatR;
using OmniSharp.Extensions.JsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public abstract class SaveHandler<T> : IJsonRpcRequestHandler<SaveRequest<T>, SaveResult>
        where T : class, new()
    {
        Task<SaveResult> IRequestHandler<SaveRequest<T>, SaveResult>.Handle(SaveRequest<T> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

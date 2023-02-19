using MediatR;
using OmniSharp.Extensions.JsonRpc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query.Asbstract
{
    public abstract class QueryHandler<T> : IJsonRpcRequestHandler<QueryRequest<T>, QueryResult<T>>
        where T : class, new()
    {
        public async Task<QueryResult<T>> Handle(QueryRequest<T> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

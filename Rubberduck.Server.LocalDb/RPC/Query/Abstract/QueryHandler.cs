using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    public abstract class QueryHandler<T> : JsonRpcRequestHandler<QueryRequest<T>, QueryResult<T>>
        where T : class, new()
    {
        protected QueryHandler(ILogger logger) : base(logger) { }
    }
}

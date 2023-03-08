using MediatR;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

namespace Rubberduck.Server.LocalDb.RPC.Query.Debug
{
    public abstract class SqlQueryRequest<TResult> : IRequest, IRequest<QueryResult<TResult>>
        where TResult : class, new()
    {
        public string RawSqlQuery { get; set; }
    }
}

using MediatR;

namespace Rubberduck.ServerPlatform.RPC.DatabaseServer
{
    public class QueryRequest<TResult, TOption> : IRequest, IRequest<QueryResult<TResult>>
        where TResult : class, new()
        where TOption : IQueryOption, new()
    {
        public TOption Options { get; set; }
    }

    public class SqlDebugQueryRequest<TResult> : IRequest, IRequest<QueryResult<TResult>>
        where TResult : class, new()
    {
        public SqlQuery DebugQuery { get; set; }
    }

    public class QueryResult<TResult> where TResult : class, new()
    {
        public IEnumerable<TResult> Results { get; set; } = Enumerable.Empty<TResult>();
    }
}

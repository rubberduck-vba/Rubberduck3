using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

namespace Rubberduck.Server.LocalDb.RPC.Query.Debug
{
    [Method(JsonRpcMethods.DatabaseServer.DebugSqlSelect, Direction.ClientToServer)]
    public class SqlQueryHandler<TResult> : JsonRpcRequestHandler<SqlQueryRequest<TResult>, QueryResult<TResult>>
        where TResult : class, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        public SqlQueryHandler(ILogger<SqlQueryHandler<TResult>> logger, IUnitOfWorkFactory factory) 
            : base(logger) 
        {
            _factory = factory;
        }

        public override Task<QueryResult<TResult>> Handle(SqlQueryRequest<TResult> request, CancellationToken cancellationToken)
        {
            return base.Handle(request, cancellationToken);
        }

        protected async override Task<QueryResult<TResult>> HandleAsync(SqlQueryRequest<TResult> request)
        {
            using (var uow = _factory.CreateNew())
            {
                var results = await uow.QueryAsync<TResult>(request.RawSqlQuery, null);
                return await Task.FromResult(new QueryResult<TResult> { Results = results });
            }
        }
    }
}
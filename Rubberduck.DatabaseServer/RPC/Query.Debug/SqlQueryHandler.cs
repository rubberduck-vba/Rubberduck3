using Microsoft.Extensions.Logging;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

#if DEBUG
namespace Rubberduck.Server.LocalDb.RPC.Query.Debug
{
    public class SqlQueryHandler<TResult> : JsonRpcRequestHandler<SqlQueryRequest<TResult>, QueryResult<TResult>>
        where TResult : class, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        public SqlQueryHandler(ILogger logger, IUnitOfWorkFactory factory) 
            : base(logger) 
        {
            _factory = factory;
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
#endif
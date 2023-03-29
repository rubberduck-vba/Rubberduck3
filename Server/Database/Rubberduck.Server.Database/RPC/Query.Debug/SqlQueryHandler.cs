using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform.Platform;
using Rubberduck.ServerPlatform.Platform.Model;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;
using Rubberduck.Server.Database.Internal.Storage.Abstract;
using System.Threading.Tasks;

#if DEBUG
namespace Rubberduck.Server.Database.RPC.Query.Debug
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
            var query = request.Params.ToObject<SqlQuery>();
            using (var uow = _factory.CreateNew())
            {
                var results = await uow.QueryAsync<TResult>(query.RawSqlSelect, null);
                return await Task.FromResult(new QueryResult<TResult> { Results = results });
            }
        }
    }
}
#endif
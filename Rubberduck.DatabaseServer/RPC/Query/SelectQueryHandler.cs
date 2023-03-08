using Microsoft.Extensions.Logging;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.DatabaseServer.RPC.Query.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

namespace Rubberduck.DatabaseServer.RPC.Query
{
    internal class SelectQueryHandler<TEntity, TOptions> : QueryHandler<TEntity, TOptions>
        where TEntity : DbEntity, new()
        where TOptions : class, IQueryOption, new()
    {
        public SelectQueryHandler(ILogger logger, IUnitOfWorkFactory factory) 
            : base(logger, factory)
        {
        }
        
        protected async override Task<QueryResult<TEntity>> HandleAsync(QueryRequest<TEntity, TOptions> request, IUnitOfWork uow, CancellationToken cancellationToken)
        {
            var results = await uow.GetRepository<TEntity>().GetByOptionsAsync(request.Options);
            return new QueryResult<TEntity> { Results = results };
        }
    }
}

using Microsoft.Extensions.Logging;
using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class SelectQueryHandler<TEntity, TOptions> : QueryHandler<TEntity, TOptions>
        where TEntity : DbEntity, new()
        where TOptions : class, new()
    {
        public SelectQueryHandler(ILogger logger, IUnitOfWorkFactory factory) 
            : base(logger, factory)
        {
        }

        protected async override Task<QueryResult<TEntity>> HandleAsync(QueryRequest<TEntity, TOptions> request, IUnitOfWork uow)
        {
            var view = uow.GetView<TEntity>();
            var results = await view.GetByOptionsAsync(request.Options);

            return new QueryResult<TEntity> { Results = results };
        }
    }
}

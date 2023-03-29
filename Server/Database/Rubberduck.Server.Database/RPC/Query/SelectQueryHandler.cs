using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform.Platform.Model;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;
using Rubberduck.Server.Database.Internal.Storage.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Server.Database.RPC.Query
{
    internal class SelectQueryHandler<TResult, TOptions> : QueryHandler<TResult, TOptions>
        where TResult : class, new()
        where TOptions : class, IQueryOption, new()
    {
        public SelectQueryHandler(ILogger logger, IUnitOfWorkFactory factory) 
            : base(logger, factory)
        {
        }

        protected override Task<QueryResult<TResult>> HandleAsync(QueryRequest<TResult, TOptions> request, IUnitOfWork uow)
        {
            throw new System.NotImplementedException();
        }
    }
}

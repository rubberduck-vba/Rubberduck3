using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query
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

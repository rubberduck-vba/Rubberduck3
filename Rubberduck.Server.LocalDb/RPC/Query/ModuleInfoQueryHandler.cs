using Microsoft.Extensions.Logging;
using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class ModuleInfoQueryHandler : QueryHandler<ModuleInfo, ModuleInfoRequestOptions>
    {
        public ModuleInfoQueryHandler(ILogger logger, IUnitOfWorkFactory factory)
            : base(logger, factory) 
        { 
        }

        protected async override Task<QueryResult<ModuleInfo>> HandleAsync(QueryRequest<ModuleInfo, ModuleInfoRequestOptions> request, IUnitOfWork uow)
        {
            var view = uow.GetView<ModuleInfo>();
            var results = await view.GetByOptionsAsync(request.Options);

            return new QueryResult<ModuleInfo> { Results = results };
        }
    }
}

using Microsoft.Extensions.Logging;
using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class ProjectInfoQueryHandler : QueryHandler<ProjectInfo, ProjectInfoRequestOptions>
    {
        public ProjectInfoQueryHandler(ILogger logger, IUnitOfWorkFactory factory)
            : base(logger, factory)
        {
        }

        protected async override Task<QueryResult<ProjectInfo>> HandleAsync(QueryRequest<ProjectInfo, ProjectInfoRequestOptions> request, IUnitOfWork uow)
        {
            var view = uow.GetView<ProjectInfo>();
            var results = await view.GetByOptionsAsync(request.Options);

            return new QueryResult<ProjectInfo> { Results = results };
        }
    }
}

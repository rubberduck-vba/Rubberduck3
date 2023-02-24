using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rubberduck.InternalApi.RPC.DataServer;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.Server.LocalDb.Internal.Storage;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class ProjectsQueryHandler : QueryHandler<Project>
    {
        private readonly ServerState _serverState;

        public ProjectsQueryHandler(ILogger logger, ServerState serverState)
            : base(logger)
        {
            _serverState = serverState;
        }

        protected override Task<QueryResult<Project>> HandleAsync(QueryRequest<Project> request)
        {
            throw new System.NotImplementedException();
        }
    }
}

using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class ProjectsRequest : QueryRequest<ProjectInfo, ProjectInfoRequestOptions>
    {
        public ProjectsRequest(object id, JToken @params)
            : base(id, JsonRpcMethods.QueryProjects, @params)
        {
        }
    }
}

using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.Database;

namespace Rubberduck.Server.Database.RPC.Query
{
    internal class ProjectsRequest : QueryRequest<ProjectInfo, ProjectInfoRequestOptions>
    {
        public ProjectsRequest(object id, JToken @params)
            : base(id, JsonRpcMethods.QueryProjects, @params)
        {
        }
    }
}

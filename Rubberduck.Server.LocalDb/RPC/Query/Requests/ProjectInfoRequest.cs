using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class ProjectInfoRequest : QueryRequest<ProjectInfo, ProjectInfoRequestOptions>
    {
        public ProjectInfoRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.QueryProjectInfo, @params)
        {
        }
    }
}

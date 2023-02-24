using Newtonsoft.Json.Linq;
using Rubberduck.Server.LocalDb.Internal.Model;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    internal class ProjectInfoRequest : QueryRequest<ProjectInfo>
    {
        public ProjectInfoRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.QueryProjectInfo, @params)
        {
        }
    }
}

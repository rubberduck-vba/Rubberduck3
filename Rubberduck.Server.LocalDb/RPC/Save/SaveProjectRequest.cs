using Newtonsoft.Json.Linq;
using Rubberduck.InternalApi.RPC.DataServer;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveProjectRequest : SaveRequest<Project>
    {
        public SaveProjectRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.SaveProject, @params)
        {
        }
    }
}

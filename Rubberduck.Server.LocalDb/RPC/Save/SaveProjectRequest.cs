using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.LocalDb;

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

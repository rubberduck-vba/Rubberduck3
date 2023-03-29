using Newtonsoft.Json.Linq;
using Rubberduck.ServerPlatform.Platform.Model.Database;

namespace Rubberduck.Server.Database.RPC.Save
{
    public class SaveModuleRequest : SaveRequest<Module>
    {
        public SaveModuleRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.SaveModule, @params)
        {
        }
    }
}

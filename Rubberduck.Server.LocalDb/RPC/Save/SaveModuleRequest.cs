using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveModuleRequest : SaveRequest<Module>
    {
        public SaveModuleRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.SaveModule, @params)
        {
        }
    }
}

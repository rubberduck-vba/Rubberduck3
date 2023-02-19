using Newtonsoft.Json.Linq;
using Rubberduck.InternalApi.RPC.DataServer;

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

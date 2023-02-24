using Newtonsoft.Json.Linq;
using Rubberduck.InternalApi.RPC.DataServer;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveLocalRequest : SaveRequest<Local>
    {
        public SaveLocalRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.SaveLocal, @params)
        {
        }
    }
}

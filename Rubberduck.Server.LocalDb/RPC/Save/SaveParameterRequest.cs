using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.Database;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveParameterRequest : SaveRequest<Parameter>
    {
        public SaveParameterRequest(object id, JToken @params)
            : base(id, JsonRpcMethods.SaveParameter, @params)
        {
        }
    }
}

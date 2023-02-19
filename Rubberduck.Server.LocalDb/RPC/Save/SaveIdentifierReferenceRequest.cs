using Newtonsoft.Json.Linq;
using Rubberduck.InternalApi.RPC.DataServer;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveIdentifierReferenceRequest : SaveRequest<IdentifierReference>
    {
        public SaveIdentifierReferenceRequest(object id, JToken @params)
            : base(id, JsonRpcMethods.SaveIdentifierReference, @params)
        {
        }
    }
}

using Newtonsoft.Json.Linq;
using Rubberduck.ServerPlatform.Platform.Model.Database;

namespace Rubberduck.Server.Database.RPC.Save
{
    public class SaveIdentifierReferenceRequest : SaveRequest<IdentifierReference>
    {
        public SaveIdentifierReferenceRequest(object id, JToken @params)
            : base(id, JsonRpcMethods.SaveIdentifierReference, @params)
        {
        }
    }
}

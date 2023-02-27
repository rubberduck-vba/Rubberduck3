using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveMemberRequest : SaveRequest<Member>
    {
        public SaveMemberRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.SaveMember, @params)
        {
        }
    }
}

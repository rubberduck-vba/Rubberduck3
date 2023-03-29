﻿using Newtonsoft.Json.Linq;
using Rubberduck.ServerPlatform.Platform.Model.Database;

namespace Rubberduck.Server.Database.RPC.Save
{
    public class SaveMemberRequest : SaveRequest<Member>
    {
        public SaveMemberRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.SaveMember, @params)
        {
        }
    }
}

using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;

namespace Rubberduck.Server.Database.RPC.Info
{
    public class InfoRequest : Request, IRequest<InfoResult>
    {
        public InfoRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.Info, @params)
        {
        }
    }
}

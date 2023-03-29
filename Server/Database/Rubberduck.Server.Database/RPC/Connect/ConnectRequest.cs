using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;

namespace Rubberduck.Server.Database.RPC.Connect
{
    public class ConnectRequest : Request, IRequest<SuccessResult>
    {
        public ConnectRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.Connect, @params)
        {
        }
    }
}

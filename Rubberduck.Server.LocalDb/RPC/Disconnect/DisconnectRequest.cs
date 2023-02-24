using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;

namespace Rubberduck.Server.LocalDb.RPC.Disconnect
{
    public class DisconnectRequest : Request, IRequest<DisconnectResult>
    {
        public DisconnectRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.Connect, @params)
        {
        }
    }
}

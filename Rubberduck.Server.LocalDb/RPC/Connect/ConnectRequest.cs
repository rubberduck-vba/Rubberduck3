using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;

namespace Rubberduck.Server.LocalDb.RPC.Connect
{
    public class ConnectRequest : Request, IRequest<ConnectResult>
    {
        public ConnectRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.Connect, @params)
        {
        }
    }
}

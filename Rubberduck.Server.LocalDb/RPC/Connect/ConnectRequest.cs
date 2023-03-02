using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database.Responses;

namespace Rubberduck.Server.LocalDb.RPC.Connect
{
    public class ConnectRequest : Request, IRequest<SuccessResult>
    {
        public ConnectRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.Database.Connect, @params)
        {
        }
    }
}

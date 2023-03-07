using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;

namespace Rubberduck.Server.LocalDb.RPC.Info
{
    public class InfoRequest : Request, IRequest<InfoResult>
    {
        public InfoRequest(object id, JToken @params) 
            : base(id, JsonRpcMethods.Database.Info, @params)
        {
        }
    }
}

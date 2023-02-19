using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;

namespace Rubberduck.Server.LocalDb.RPC.Initialize
{
    public class InitializeRequest : Request, IRequest<InitializeResult>
    {
        public InitializeRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }
    }
}

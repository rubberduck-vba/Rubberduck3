using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public abstract class SaveRequest<T> : Request, IRequest<SaveResult>
        where T : class, new()
    {
        public SaveRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }
    }
}

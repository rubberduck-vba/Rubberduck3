using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.Server.LocalDb.Internal.Model;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public abstract class SaveRequest<TEntity> : Request, IRequest<SaveResult>
        where TEntity : DbEntity, new()
    {
        public SaveRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }
    }
}

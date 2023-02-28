using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.RPC.Platform.Model.Database.Responses;

namespace Rubberduck.Server.Database.RPC.Save
{
    public abstract class SaveRequest<TEntity> : Request, IRequest<SuccessResult>
        where TEntity : DbEntity, new()
    {
        public SaveRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }
    }
}

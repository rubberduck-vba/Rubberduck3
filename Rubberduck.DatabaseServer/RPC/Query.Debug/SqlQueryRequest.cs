using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.RPC.Platform.Model.Database.Responses;

namespace Rubberduck.Server.LocalDb.RPC.Query.Debug
{
    public abstract class SqlQueryRequest<TResult> : Request, IRequest<QueryResult<TResult>>
        where TResult : class, new()
    {
        public SqlQueryRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }
    }
}

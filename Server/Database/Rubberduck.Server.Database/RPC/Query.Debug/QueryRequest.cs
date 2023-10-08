using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.ServerPlatform.Platform.Model;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;

namespace Rubberduck.Server.Database.RPC.Query
{
    public abstract class QueryRequest<TResult, TOption> : Request, IRequest<QueryResult<TResult>>
        where TResult : class, new()
        where TOption : IQueryOption, new()
    {
        public QueryRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }

        public virtual TOption Options { get; set; }
    }
}

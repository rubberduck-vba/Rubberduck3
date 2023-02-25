using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    public abstract class QueryRequest<TResult, TOptions> : Request, IRequest<QueryResult<TResult>>
        where TResult : class, new()
        where TOptions : class, new()
    {
        public QueryRequest(object id, string method, JToken @params) 
            : base(id, method, @params)
        {
        }

        public TOptions Options { get; set; } = new TOptions();
    }
}

using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using System.Text;

namespace Rubberduck.Server.LocalDb.RPC.Query.Asbstract
{
    public abstract class QueryRequest<T> : Request, IRequest<QueryResult<T>>
        where T : class, new()
    {
        public QueryRequest(object id, string method, JToken @params) : base(id, method, @params)
        {
        }
    }
}

using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using Rubberduck.Server.LocalDb.Internal.Model;
using System.Text;

namespace Rubberduck.Server.LocalDb.RPC.Query
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

using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    public class QueryResult<TResult> where TResult : class, new()
    {
        public IEnumerable<TResult> Results { get; set; } = Enumerable.Empty<TResult>();
    }
}

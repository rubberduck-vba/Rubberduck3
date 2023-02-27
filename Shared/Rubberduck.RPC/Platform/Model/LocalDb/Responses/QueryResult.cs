using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.RPC.Platform.Model.LocalDb.Responses
{
    public class QueryResult<TResult> where TResult : class, new()
    {
        public IEnumerable<TResult> Results { get; set; } = Enumerable.Empty<TResult>();
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Server.LocalDb.RPC.Query
{
    public class QueryResult<T> where T : class, new()
    {
        public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();
    }
}

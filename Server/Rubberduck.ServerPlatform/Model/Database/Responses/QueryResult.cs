using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.ServerPlatform.Model.LocalDb.Responses
{
    public class QueryResult<TResult> where TResult : class, new()
    {
        public IEnumerable<TResult> Results { get; set; } = Enumerable.Empty<TResult>();
    }
}

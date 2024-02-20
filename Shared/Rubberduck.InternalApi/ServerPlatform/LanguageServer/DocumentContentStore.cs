using System.Linq;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public class DocumentContentStore : ConcurrentContentStore<DocumentState> 
{
    public IOrderedEnumerable<DocumentState> Enumerate()
    {
        return Store.Values.OrderByDescending(e => e.IsOpened);
    }
}
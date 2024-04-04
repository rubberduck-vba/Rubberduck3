using System.Collections.Generic;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface IComCollection<out TItem> : IEnumerable<TItem>
    {
        int Count { get; }
        TItem this[object index] { get; }
    }
}
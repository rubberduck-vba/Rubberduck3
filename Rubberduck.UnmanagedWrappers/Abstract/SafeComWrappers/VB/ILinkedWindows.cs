using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface ILinkedWindows : ISafeComWrapper, IComCollection<IWindow>, IEquatable<ILinkedWindows>
    {
        IVBE VBE { get; }
        IWindow Parent { get; }
        void Remove(IWindow window);
        void Add(IWindow window);
    }
}
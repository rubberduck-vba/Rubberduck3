using Rubberduck.Unmanaged.Events;
using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IReferences : ISafeEventedComWrapper, IComCollection<IReference>, IEquatable<IReferences>
    {
        event EventHandler<ReferenceEventArgs> ItemAdded;
        event EventHandler<ReferenceEventArgs> ItemRemoved;

        IVBE VBE { get; }
        IVBProject Parent { get; }

        IReference AddFromGuid(string guid, int major, int minor);
        IReference AddFromFile(string path);
        void Remove(IReference reference);
    }
}
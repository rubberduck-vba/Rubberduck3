using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface IAddIns : ISafeComWrapper, IComCollection<IAddIn>, IEquatable<IAddIns>
    {
        object Parent { get; }
        IVBE VBE { get; }
        void Update();
    }
}
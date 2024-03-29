using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IProperties : ISafeComWrapper, IComCollection<IProperty>, IEquatable<IProperties>
    {
        IVBE VBE { get; }
        IApplication Application { get; }
        object Parent { get; }

    }
}
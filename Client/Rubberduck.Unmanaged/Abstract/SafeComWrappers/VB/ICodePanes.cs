using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface ICodePanes : ISafeComWrapper, IComCollection<ICodePane>, IEquatable<ICodePanes>
    {
        IVBE Parent { get; }
        IVBE VBE { get; }
        ICodePane Current { get; set; }
    }
}
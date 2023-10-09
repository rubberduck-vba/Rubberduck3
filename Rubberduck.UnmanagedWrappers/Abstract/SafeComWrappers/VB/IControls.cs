using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IControls : ISafeComWrapper, IComCollection<IControl>, IEquatable<IControls>
    {

    }
}
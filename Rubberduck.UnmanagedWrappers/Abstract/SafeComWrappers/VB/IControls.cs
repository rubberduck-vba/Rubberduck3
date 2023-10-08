using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface IControls : ISafeComWrapper, IComCollection<IControl>, IEquatable<IControls>
    {
        
    }
}
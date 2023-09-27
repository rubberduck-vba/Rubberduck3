using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    public interface IApplication : ISafeComWrapper, IEquatable<IApplication>
    {
        string Version { get; }
    }
}
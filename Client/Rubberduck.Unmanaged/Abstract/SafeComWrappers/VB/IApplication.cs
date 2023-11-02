using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IApplication : ISafeComWrapper, IEquatable<IApplication>
    {
        string Version { get; }
    }
}
using System;

namespace Rubberduck.Unmanaged.TypeLibs.Abstract
{
    public interface ITypeLibReference
    {
        string RawString { get; }
        Guid GUID { get; }
        uint MajorVersion { get; }
        uint MinorVersion { get; }
        uint LCID { get; }
        string Path { get; }
        string Name { get; }
        ITypeLibWrapper TypeLib { get; }
    }
}
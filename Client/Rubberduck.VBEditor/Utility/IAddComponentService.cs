using Rubberduck.Unmanaged.Model;
using System;

namespace Rubberduck.VBEditor.Utility
{
    public interface IAddComponentService
    {
        void AddComponent(Uri workspaceUri, ComponentType componentType, string? code = null, string? additionalPrefixInModule = null, string? componentName = null);
        void AddComponentWithAttributes(Uri workspaceUri, ComponentType componentType, string code, string? prefixInModule = null, string? componentName = null);
    }
}
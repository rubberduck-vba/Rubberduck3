using System.Collections.Generic;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.VBEditor.Utility
{
    public interface IRequiredBinaryFilesFromFileNameExtractor
    {
        ICollection<ComponentType> SupportedComponentTypes { get; }
        ICollection<string> RequiredBinaryFiles(string fileName, ComponentType componentType);
    }
}
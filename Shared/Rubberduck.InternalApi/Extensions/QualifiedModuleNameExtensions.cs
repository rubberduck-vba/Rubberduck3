using Rubberduck.Unmanaged.Model.Abstract;
using System.IO;

namespace Rubberduck.InternalApi.Extensions;

public static class QualifiedModuleNameExtensions
{
    public static WorkspaceFileUri ToWorkspaceFileUri(this IQualifiedModuleName source, string? relativePath = null)
    {
        relativePath = Path.Combine(relativePath ?? string.Empty, source.Name + source.ComponentType.FileExtension());
        return new WorkspaceFileUri(relativePath, source.WorkspaceUri);
    }
}

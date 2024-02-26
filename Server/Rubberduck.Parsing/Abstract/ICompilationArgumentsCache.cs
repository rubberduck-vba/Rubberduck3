using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Parsing.Abstract;

public interface ICompilationArgumentsCache : ICompilationArgumentsProvider
{
    void ReloadCompilationArguments(IEnumerable<WorkspaceUri> workspaceUris);
    IReadOnlyCollection<WorkspaceUri> ProjectWhoseCompilationArgumentsChanged();
    void ClearProjectWhoseCompilationArgumentsChanged();
    void RemoveCompilationArgumentsFromCache(IEnumerable<WorkspaceUri> workspaceUris);
}

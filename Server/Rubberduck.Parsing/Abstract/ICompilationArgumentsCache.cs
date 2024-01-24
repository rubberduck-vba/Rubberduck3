namespace Rubberduck.Parsing.Abstract;

public interface ICompilationArgumentsCache : ICompilationArgumentsProvider
{
    void ReloadCompilationArguments(IEnumerable<Uri> workspaceUris);
    IReadOnlyCollection<Uri> ProjectWhoseCompilationArgumentsChanged();
    void ClearProjectWhoseCompilationArgumentsChanged();
    void RemoveCompilationArgumentsFromCache(IEnumerable<Uri> workspaceUris);
}

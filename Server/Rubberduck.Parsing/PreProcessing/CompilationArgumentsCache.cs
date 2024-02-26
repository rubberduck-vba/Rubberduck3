using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.PreProcessing;

public class CompilationArgumentsCache : ICompilationArgumentsCache
{
    private readonly ICompilationArgumentsProvider _provider;
    private readonly Dictionary<Uri, Dictionary<string, short>> _compilationArguments = [];
    private readonly HashSet<WorkspaceUri> _projectsWhoseCompilationArgumentsChanged = [];

    public CompilationArgumentsCache(ICompilationArgumentsProvider compilationArgumentsProvider)
    {
        _provider = compilationArgumentsProvider;
    }

    public VBAPredefinedCompilationConstants PredefinedCompilationConstants => _provider.PredefinedCompilationConstants;

    public Dictionary<string, short> UserDefinedCompilationArguments(WorkspaceUri workspaceUri)
    {
        return _compilationArguments.TryGetValue(workspaceUri.WorkspaceRoot, out var args) ? args : [];
    }

    public void ReloadCompilationArguments(IEnumerable<WorkspaceUri> workspaceUris)
    {
        foreach (var uri in workspaceUris)
        {
            var oldCompilationArguments = UserDefinedCompilationArguments(uri);
            ReloadCompilationArguments(uri);

            var newCompilationArguments = UserDefinedCompilationArguments(uri);
            if (!newCompilationArguments.HasEqualContent(oldCompilationArguments))
            {
                _projectsWhoseCompilationArgumentsChanged.Add(uri);
            }
        }
    }

    private void ReloadCompilationArguments(WorkspaceUri workspaceUri)
    {
        _compilationArguments[workspaceUri] = _provider.UserDefinedCompilationArguments(workspaceUri);
    }

    public IReadOnlyCollection<WorkspaceUri> ProjectWhoseCompilationArgumentsChanged()
    {
        return _projectsWhoseCompilationArgumentsChanged;
    }

    public void ClearProjectWhoseCompilationArgumentsChanged()
    {
        _projectsWhoseCompilationArgumentsChanged.Clear();
    }

    public void RemoveCompilationArgumentsFromCache(IEnumerable<WorkspaceUri> workspaceUris)
    {
        foreach (var uri in workspaceUris)
        {
            _compilationArguments.Remove(uri);
        }
    }
}

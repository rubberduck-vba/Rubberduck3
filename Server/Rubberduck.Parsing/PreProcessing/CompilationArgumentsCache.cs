using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.PreProcessing;

public class CompilationArgumentsCache : ICompilationArgumentsCache
{
    private readonly ICompilationArgumentsProvider _provider;
    private readonly Dictionary<Uri, Dictionary<string, short>> _compilationArguments = [];
    private readonly HashSet<Uri> _projectsWhoseCompilationArgumentsChanged = [];

    public CompilationArgumentsCache(ICompilationArgumentsProvider compilationArgumentsProvider)
    {
        _provider = compilationArgumentsProvider;
    }

    public VBAPredefinedCompilationConstants PredefinedCompilationConstants => _provider.PredefinedCompilationConstants;

    public Dictionary<string, short> UserDefinedCompilationArguments(Uri workspaceUri)
    {
        return _compilationArguments.TryGetValue(workspaceUri, out var args) ? args : [];
    }

    public void ReloadCompilationArguments(IEnumerable<Uri> workspaceUris)
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

    private void ReloadCompilationArguments(Uri workspaceUri)
    {
        _compilationArguments[workspaceUri] = _provider.UserDefinedCompilationArguments(workspaceUri);
    }

    public IReadOnlyCollection<Uri> ProjectWhoseCompilationArgumentsChanged()
    {
        return _projectsWhoseCompilationArgumentsChanged;
    }

    public void ClearProjectWhoseCompilationArgumentsChanged()
    {
        _projectsWhoseCompilationArgumentsChanged.Clear();
    }

    public void RemoveCompilationArgumentsFromCache(IEnumerable<Uri> workspaceUris)
    {
        foreach (var uri in workspaceUris)
        {
            _compilationArguments.Remove(uri);
        }
    }
}

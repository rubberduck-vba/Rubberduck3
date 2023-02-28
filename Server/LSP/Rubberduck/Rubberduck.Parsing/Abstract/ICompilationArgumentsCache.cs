using System.Collections.Generic;

namespace Rubberduck.Parsing.Abstract
{
    public interface ICompilationArgumentsCache : ICompilationArgumentsProvider
    {
        void ReloadCompilationArguments(IEnumerable<string> projectIds);
        IReadOnlyCollection<string> ProjectWhoseCompilationArgumentsChanged();
        void ClearProjectWhoseCompilationArgumentsChanged();
        void RemoveCompilationArgumentsFromCache(IEnumerable<string> projectIds);
    }
}

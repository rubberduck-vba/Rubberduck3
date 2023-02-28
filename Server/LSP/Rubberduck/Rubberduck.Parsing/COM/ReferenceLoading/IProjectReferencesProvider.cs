using System.Collections.Generic;

namespace Rubberduck.Parsing.COM.ReferenceLoading
{
    public interface IProjectReferencesProvider
    {
        IReadOnlyCollection<ReferencePriorityMap> ProjectReferences { get; }
    }

}

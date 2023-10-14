using System.Collections.Generic;

namespace Rubberduck.Parsing.COM.Abstract
{
    public interface IUserComProjectRepository : IUserComProjectProvider
    {
        void RefreshUserComProjects(IReadOnlyCollection<string> projectIdsToReload);
    }
}
using Rubberduck.Parsing.Model.ComReflection;
using System.Collections.Generic;

namespace Rubberduck.Parsing.COM.Abstract
{
    public interface IUserComProjectProvider
    {
        ComProject UserProject(string projectId);
        IReadOnlyDictionary<string, ComProject> UserProjects();
    }
}
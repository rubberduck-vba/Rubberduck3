using Rubberduck.InternalApi.Model;
using System.Collections.Generic;

namespace Rubberduck.UI.NewProject
{
    public interface ITemplatesService
    {
        void DeleteTemplate(string name);
        void SaveAsTemplate(ProjectFile projectFile);
        IEnumerable<ProjectTemplate> GetProjectTemplates();
        ProjectTemplate Resolve(ProjectTemplate template);
    }
}

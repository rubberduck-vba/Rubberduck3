using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Command;
using System.Collections.Generic;

namespace Rubberduck.UI.NewProject
{
    public interface INewProjectWindowViewModel : IBrowseFolderModel, ICommandBindingProvider
    {
        IEnumerable<VBProjectInfo?> VBProjects { get; }
        VBProjectInfo? SelectedVBProject { get; }
        IEnumerable<ProjectTemplate> ProjectTemplates { get; }
        ProjectTemplate? SelectedProjectTemplate { get; }

        string ProjectName { get; set; }
        string WorkspaceLocation { get; set; }
    }
}

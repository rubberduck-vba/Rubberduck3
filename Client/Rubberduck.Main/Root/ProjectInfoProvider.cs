//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Services.NewProject;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Rubberduck.Main.Root
{
    class ProjectInfoProvider : IVBProjectInfoProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProjectsRepository _projects;

        public ProjectInfoProvider(IFileSystem fileSystem, IProjectsRepository projects)
        {
            _fileSystem = fileSystem;
            _projects = projects;
        }

        public IEnumerable<VBProjectInfo?> GetProjectInfo()
        {
            _projects.Refresh();
            var result = new List<VBProjectInfo?>();
            foreach (var e in _projects.Projects())
            {
                result.Add(GetProjectInfo(e.ProjectId, e.Project));
            }
            return result;
        }

        private VBProjectInfo GetProjectInfo(string id, IVBProject project)
        {
            var fullpath = project.FileName;
            var directory = _fileSystem.DirectoryInfo.New(fullpath[..^(_fileSystem.Path.GetFileName(fullpath).Length)]);
            var projectFile = directory.GetFiles(ProjectFile.FileName).SingleOrDefault();

            return new VBProjectInfo
            {
                ProjectId = id,
                Name = project.Name,
                IsLocked = project.Protection == Unmanaged.Abstract.SafeComWrappers.VB.Enums.ProjectProtection.Unprotected,
                Location = directory.FullName,
                HasWorkspace = projectFile != null
            };
        }
    }
}

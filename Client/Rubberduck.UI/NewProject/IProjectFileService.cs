using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using System;
using System.IO.Abstractions;
using System.Text.Json;

namespace Rubberduck.UI.NewProject
{
    public interface IProjectFileService
    {
        void CreateFile(ProjectFile model);
        ProjectFile ReadFile(Uri root);
    }

    public interface IWorkspaceFolderService
    {
        void CreateWorkspace(string path);
    }

    public class ProjectFileService : ServiceBase, IProjectFileService
    {
        private readonly IFileSystem _fileSystem;

        public ProjectFileService(ILogger<ProjectFileService> logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem) 
            : base(logger, settingsProvider)
        {
            _fileSystem = fileSystem;
        }

        public void CreateFile(ProjectFile model)
        {
            var path = _fileSystem.Path.Combine(model.Uri.LocalPath, model.VBProject.Name, ProjectFile.FileName);
            var content = JsonSerializer.Serialize(model);
            _fileSystem.File.WriteAllText(path, content);
        }

        public ProjectFile ReadFile(Uri root)
        {
            var path = _fileSystem.Path.Combine(root.LocalPath, ProjectFile.FileName);
            var content = _fileSystem.File.ReadAllText(path);
            return JsonSerializer.Deserialize<ProjectFile>(content)
                ?? throw new InvalidOperationException();
        }
    }

    public class WorkspaceFolderService : ServiceBase, IWorkspaceFolderService
    {
        private readonly IFileSystem _fileSystem;

        public WorkspaceFolderService(ILogger<WorkspaceFolderService> logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem) 
            : base(logger, settingsProvider)
        {
            _fileSystem = fileSystem;
        }

        public void CreateWorkspace(string path)
        {
            _fileSystem.Directory.CreateDirectory(path);

            var sourceRoot = _fileSystem.Path.Combine(path, ProjectFile.SourceRoot);
            _fileSystem.Directory.CreateDirectory(sourceRoot);
        }
    }
}

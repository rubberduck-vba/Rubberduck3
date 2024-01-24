using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Settings;
using System.IO.Abstractions;
using System.Linq;

namespace Rubberduck.InternalApi.Services
{
    public class WorkspaceFolderService : ServiceBase, IWorkspaceFolderService
    {
        private readonly IFileSystem _fileSystem;

        public WorkspaceFolderService(ILogger<WorkspaceFolderService> logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem, PerformanceRecordAggregator performance)
            : base(logger, settingsProvider, performance)
        {
            _fileSystem = fileSystem;
        }

        public void CopyTemplateFiles(ProjectFile projectFile, string templateSourceRoot)
        {
            foreach (var file in projectFile.VBProject.AllFiles)
            {
                var sourcePath = _fileSystem.Path.Combine(templateSourceRoot, file.Uri);
                var destinationUri = new WorkspaceFileUri(file.Uri, projectFile.Uri);

                _fileSystem.Directory.CreateDirectory(destinationUri.WorkspaceFolder.AbsoluteLocation.LocalPath);
                _fileSystem.File.Copy(sourcePath, destinationUri.AbsoluteLocation.LocalPath, overwrite: true);
            }
        }

        public void CreateWorkspaceFolders(ProjectFile projectFile)
        {
            var workspaceRoot = projectFile.Uri;
            _fileSystem.Directory.CreateDirectory(workspaceRoot.LocalPath);

            var sourceRoot = _fileSystem.Path.Combine(workspaceRoot.LocalPath, ProjectFile.SourceRoot);
            _fileSystem.Directory.CreateDirectory(sourceRoot);

            var folders = projectFile.VBProject.Modules
                .Select(e => new WorkspaceFileUri(e.Uri, workspaceRoot).WorkspaceFolder)
                .ToHashSet()
                .Select(Folder.FromWorkspaceUri);

            projectFile.VBProject.Folders = folders.ToArray();

            foreach (var folder in projectFile.VBProject.Folders)
            {
                var path = folder.GetWorkspaceUri(workspaceRoot).AbsoluteLocation.LocalPath;
                _fileSystem.Directory.CreateDirectory(path);
            }
        }
    }
}

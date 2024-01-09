using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;

namespace Rubberduck.UI.Services.Abstract
{
    public interface IWorkspaceState
    {
        Uri? WorkspaceRoot { get; set; }
        string ProjectName { get; set; }
        IEnumerable<WorkspaceFileInfo> WorkspaceFiles { get; }

        /// <summary>
        /// Attempts to retrieve the specified file.
        /// </summary>
        /// <param name="uri">The URI referring to the file to retrieve.</param>
        /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
        /// <returns><c>true</c> if the specified version was found.</returns>
        bool TryGetWorkspaceFile(WorkspaceFileUri uri, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Marks the file at the specified URI as closed in the editor.
        /// </summary>
        /// <param name="uri">The URI referring to the file to mark as closed.</param>
        /// <param name="fileInfo">Holds a non-null reference if the file was found.</param>
        /// <returns><c>true</c> if the workspace file was correctly found and marked as closed.</returns>
        bool CloseWorkspaceFile(WorkspaceFileUri uri, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Loads the specified file into the workspace.
        /// </summary>
        /// <param name="file">The file (including its content) to be added.</param>
        /// <returns><c>true</c> if the file was successfully added to the workspace.</returns>
        /// <remarks>This method will overwrite a cached URI if URI matches an existing file.</remarks>
        bool LoadWorkspaceFile(WorkspaceFileInfo file);
        /// <summary>
        /// Renames the specified workspace URI.
        /// </summary>
        /// <param name="oldUri">The old URI.</param>
        /// <param name="newUri">The new URI.</param>
        /// <returns><c>true</c> if the rename was successful.</returns>
        bool RenameWorkspaceFile(WorkspaceFileUri oldUri, WorkspaceFileUri newUri);
        /// <summary>
        /// Unloads the specified workspace URI.
        /// </summary>
        /// <param name="uri">The file URI to unload.</param>
        /// <returns><c>true</c> if the file was successfully unloaded.</returns>
        bool UnloadWorkspaceFile(WorkspaceFileUri uri);
        void UnloadAllFiles();
    }

    public interface IWorkspaceStateManager
    {
        IEnumerable<IWorkspaceState> Workspaces { get; }
        /// <summary>
        /// Gets the currently selected/active workspace/project.
        /// </summary>
        IWorkspaceState? ActiveWorkspace { get; }
        IWorkspaceState AddWorkspace(Uri workspaceRoot);
        void Unload(Uri workspaceRoot);
    }

    public interface IProjectFileService
    {
        void CreateFile(ProjectFile model);
        ProjectFile ReadFile(Uri root);
    }

    public interface IWorkspaceFolderService
    {
        void CreateWorkspaceFolders(ProjectFile projectFile);
        void CopyTemplateFiles(ProjectFile projectFile, string templateSourceRoot);
    }

    public class ProjectFileService : ServiceBase, IProjectFileService
    {
        private readonly IFileSystem _fileSystem;

        public ProjectFileService(ILogger<ProjectFileService> logger, RubberduckSettingsProvider settingsProvider,
            IFileSystem fileSystem, PerformanceRecordAggregator performance)
            : base(logger, settingsProvider, performance)
        {
            _fileSystem = fileSystem;
        }

        public void CreateFile(ProjectFile model)
        {
            var path = _fileSystem.Path.Combine(model.Uri.LocalPath, ProjectFile.FileName);
            var content = JsonSerializer.Serialize(model);

            _fileSystem.File.WriteAllText(path, content);
        }

        public ProjectFile ReadFile(Uri root)
        {
            var path = _fileSystem.Path.Combine(root.LocalPath, ProjectFile.FileName);
            var content = _fileSystem.File.ReadAllText(path);
            var projectFile = JsonSerializer.Deserialize<ProjectFile>(content) ?? throw new InvalidOperationException();

            projectFile.Uri = root;
            return projectFile;
        }
    }

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

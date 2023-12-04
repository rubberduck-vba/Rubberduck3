using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.Json;

namespace Rubberduck.UI.Services.Abstract
{
    /// <summary>
    /// Manages the state and content of each file in a workspace.
    /// </summary>
    public interface IWorkspaceStateManager
    {
        Uri? WorkspaceRoot { get; set; }
        IEnumerable<WorkspaceFileInfo> WorkspaceFiles { get; }
        void ClearPreviousVersions(Uri uri);
        /// <summary>
        /// Attempts to retrieve the specified file.
        /// </summary>
        /// <param name="uri">The URI referring to the file to retrieve.</param>
        /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
        /// <returns><c>true</c> if the specified version was found.</returns>
        bool TryGetWorkspaceFile(Uri uri, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Attempts to retrieve the specified version of the specified file.
        /// </summary>
        /// <param name="uri">The URI referring to the file to retrieve.</param>
        /// <param name="version">The specific version number to retrieve.</param>
        /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
        /// <returns><c>true</c> if the specified version was found.</returns>
        bool TryGetWorkspaceFile(Uri uri, int version, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Marks the file at the specified URI as closed in the editor.
        /// </summary>
        /// <param name="uri">The URI referring to the file to mark as closed.</param>
        /// <param name="fileInfo">Holds a non-null reference if the file was found.</param>
        /// <returns><c>true</c> if the workspace file was correctly found and marked as closed.</returns>
        bool CloseWorkspaceFile(Uri uri, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Loads the specified file into the workspace.
        /// </summary>
        /// <param name="file">The file (including its content) to be added.</param>
        /// <param name="cacheCapacity">The number of versions held in cache.</param>
        /// <returns><c>true</c> if the file was successfully added to the workspace.</returns>
        /// <remarks>This method will overwrite a cached URI for a newer version if the content is different.</remarks>
        bool LoadWorkspaceFile(WorkspaceFileInfo file, int cacheCapacity = 3);
        /// <summary>
        /// Renames the specified workspace URI.
        /// </summary>
        /// <param name="oldUri">The old URI.</param>
        /// <param name="newUri">The new URI.</param>
        /// <returns><c>true</c> if the rename was successful.</returns>
        bool RenameWorkspaceFile(Uri oldUri, Uri newUri);
        /// <summary>
        /// Unloads the specified workspace URI.
        /// </summary>
        /// <param name="uri">The file URI to unload.</param>
        /// <returns><c>true</c> if the file was successfully unloaded.</returns>
        bool UnloadWorkspaceFile(Uri uri);
        /// <summary>
        /// Unloads the entire workspace.
        /// </summary>
        void UnloadWorkspace();
    }

    public interface IProjectFileService
    {
        void CreateFile(ProjectFile model);
        ProjectFile ReadFile(Uri root);
    }

    public interface IWorkspaceFolderService
    {
        void CreateWorkspaceFolders(ProjectFile projectFile, string workspaceRoot);
        void CopyTemplateFiles(ProjectFile projectFile, string workspaceSourceRoot, string templateSourceRoot);
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
            var path = _fileSystem.Path.Combine(model.Uri.LocalPath, model.VBProject.Name, ProjectFile.FileName);
            var content = JsonSerializer.Serialize(model);
            _fileSystem.File.WriteAllText(path, content);
        }

        public ProjectFile ReadFile(Uri root)
        {
            var path = _fileSystem.Path.Combine(root.LocalPath, ProjectFile.FileName);
            var content = _fileSystem.File.ReadAllText(path);
            var projectFile = JsonSerializer.Deserialize<ProjectFile>(content) ?? throw new InvalidOperationException();
            projectFile.Uri = new Uri(path);
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

        public void CopyTemplateFiles(ProjectFile projectFile, string workspaceSourceRoot, string templateSourceRoot)
        {
            foreach (var file in projectFile.VBProject.AllFiles)
            {
                var sourcePath = _fileSystem.Path.Combine(templateSourceRoot, file.Uri);
                var destinationPath = _fileSystem.Path.Combine(workspaceSourceRoot, file.Uri);

                var folder = _fileSystem.Path.GetDirectoryName(destinationPath)!;
                _fileSystem.Directory.CreateDirectory(folder);
                _fileSystem.File.Copy(sourcePath, destinationPath, overwrite: true);
            }
        }

        public void CreateWorkspaceFolders(ProjectFile projectFile, string workspaceRoot)
        {
            _fileSystem.Directory.CreateDirectory(workspaceRoot);

            var sourceRoot = _fileSystem.Path.Combine(workspaceRoot, ProjectFile.SourceRoot);
            _fileSystem.Directory.CreateDirectory(sourceRoot);

            foreach (var folder in projectFile.VBProject.Folders)
            {
                _fileSystem.Directory.CreateDirectory(folder.Uri);
            }
        }
    }
}

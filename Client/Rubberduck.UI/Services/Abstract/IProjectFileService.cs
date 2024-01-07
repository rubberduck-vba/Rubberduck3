using Microsoft.Extensions.Logging;
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
        bool TryGetWorkspaceFile(Uri uri, out WorkspaceFileInfo? fileInfo);
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
        /// <returns><c>true</c> if the file was successfully added to the workspace.</returns>
        /// <remarks>This method will overwrite a cached URI if URI matches an existing file.</remarks>
        bool LoadWorkspaceFile(WorkspaceFileInfo file);
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
            var path = _fileSystem.Path.Combine(model.Uri.LocalPath, ProjectFile.FileName);
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

            var folders = projectFile.VBProject.Modules.Select(e => _fileSystem.Path.Combine(workspaceRoot, e.Uri[..^(_fileSystem.Path.GetFileName(e.Uri).Length + 1)])).ToHashSet();

            projectFile.VBProject.Folders = folders
                .Select(folder => new Folder 
                    { 
                        Name = folder.TrimStart(_fileSystem.Path.DirectorySeparatorChar).Replace(_fileSystem.Path.DirectorySeparatorChar, '.'), 
                        Uri = folder 
                    })
                .ToArray();

            foreach (var folder in projectFile.VBProject.Folders)
            {
                var path = _fileSystem.Path.Combine(workspaceRoot, ProjectFile.SourceRoot + _fileSystem.Path.DirectorySeparatorChar + folder.Uri.TrimStart(_fileSystem.Path.DirectorySeparatorChar));
                _fileSystem.Directory.CreateDirectory(path);
            }
        }
    }
}

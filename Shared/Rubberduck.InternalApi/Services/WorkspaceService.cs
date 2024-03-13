using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.Services
{
    public class WorkspaceService : ServiceBase, IWorkspaceService, IDisposable
    {
        private readonly IWorkspaceStateManager _state;
        private readonly HashSet<ProjectFile> _projectFiles = new();

        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileService _projectFile;
        private readonly List<Reference> _references = [];

        private Task? _lspStartupTask = null;

        public event EventHandler<WorkspaceServiceEventArgs> WorkspaceOpened = delegate { };
        public event EventHandler<WorkspaceServiceEventArgs> WorkspaceClosed = delegate { };

        public WorkspaceService(ILogger<WorkspaceService> logger, RubberduckSettingsProvider settingsProvider,
            IWorkspaceStateManager state, IFileSystem fileSystem, PerformanceRecordAggregator performance,
            IProjectFileService projectFile)
            : base(logger, settingsProvider, performance)
        {
            _state = state;
            _fileSystem = fileSystem;
            _projectFile = projectFile;
        }

        public IWorkspaceStateManager State => _state;

        public async virtual Task OnWorkspaceOpenedAsync(Uri uri) => OnWorkspaceOpened(uri);
        protected void OnWorkspaceOpened(Uri uri) => WorkspaceOpened?.Invoke(this, new(uri));

        public void OnWorkspaceClosed(Uri uri) => WorkspaceClosed(this, new(uri));

        public IFileSystem FileSystem => _fileSystem;

        public IEnumerable<ProjectFile> ProjectFiles => _projectFiles;

        public async Task<bool> OpenProjectWorkspaceAsync(Uri uri)
        {
            var result = await Task.Run(() =>
            {
                if (!TryRunAction(() =>
                {
                    var root = uri.LocalPath;
                    var projectFilePath = _fileSystem.Path.Combine(root, ProjectFile.FileName);
                    if (!_fileSystem.File.Exists(projectFilePath))
                    {
                        throw new FileNotFoundException("No project file ('.rdproj') was found under the specified workspace URI.");
                    }

                    var projectFile = _projectFile.ReadFile(uri);
                    var rdprojVersion = new Version(projectFile.Rubberduck);
                    var rdVersion = new Version(ProjectFile.RubberduckVersion);

                    if (rdprojVersion > new Version(ProjectFile.RubberduckVersion))
                    {
                        throw new NotSupportedException("This project was created with a version of Rubberduck greater than the one currently running.");
                    }

                    var sourceRoot = _fileSystem.Path.Combine(root, WorkspaceUri.SourceRootName);
                    if (!_fileSystem.Directory.Exists(sourceRoot))
                    {
                        throw new DirectoryNotFoundException("Project source root folder ('.src') was not found under the secified workspace URI.");
                    }

                    var state = _state.AddWorkspace(uri);
                    state.ProjectName = _fileSystem.Path.GetFileName(root);
                    
                    foreach (var reference in projectFile.VBProject.References)
                    {
                        state.AddReference(reference);
                    }

                    LoadWorkspaceFiles(uri, projectFile);
                    _projectFiles.Add(projectFile);

                }, out var exception) && exception is not null)
                {
                    LogException(exception);
                    return false;
                }
                else
                {
                    return true;
                }
            });

            if (result && _lspStartupTask is null)
            {
                _lspStartupTask = OnWorkspaceOpenedAsync(uri);
            }

            return result;
        }

        public bool IsReady => _lspStartupTask?.IsCompletedSuccessfully ?? false;

        public async Task<bool> SaveWorkspaceFileAsync(WorkspaceFileUri uri)
        {
            var workspace = _state.ActiveWorkspace;
            if (workspace?.WorkspaceRoot != null && workspace.TryGetWorkspaceFile(uri, out var file) && file != null)
            {
                var path = _fileSystem.Path.Combine(workspace.WorkspaceRoot.LocalPath, WorkspaceUri.SourceRootName, file.Uri.LocalPath);
                await _fileSystem.File.WriteAllTextAsync(path, file.Text);
                return true;
            }

            return false;
        }

        public async Task<bool> SaveWorkspaceFileAsAsync(WorkspaceFileUri uri, string path)
        {
            var workspace = _state.ActiveWorkspace;
            if (workspace?.WorkspaceRoot != null && workspace.TryGetWorkspaceFile(uri, out var file) && file != null)
            {
                // note: saves a copy but only keeps the original URI in the workspace
                await _fileSystem.File.WriteAllTextAsync(path, file.Text);
                return true;
            }

            return false;
        }

        public async Task<bool> SaveAllAsync()
        {
            var tasks = new List<Task>();
            var workspace = _state.ActiveWorkspace;
            if (workspace?.WorkspaceRoot != null)
            {
                var srcRoot = _fileSystem.Path.Combine(workspace.WorkspaceRoot.LocalPath, WorkspaceUri.SourceRootName);
                foreach (var file in workspace.WorkspaceFiles.Where(e => e.IsModified).ToArray())
                {
                    var path = _fileSystem.Path.Combine(srcRoot, file.Uri.ToString());
                    tasks.Add(_fileSystem.File.WriteAllTextAsync(path, file.Text));

                    if (!workspace.SaveWorkspaceFile(file.Uri))
                    {
                        LogWarning("Could not reset document version.", file.Uri.ToString());
                    }
                }
            }

            return await Task.WhenAll(tasks).ContinueWith(t => !t.IsFaulted, TaskScheduler.Current);
        }

        private void LoadWorkspaceFiles(Uri workspaceRoot, ProjectFile projectFile)
        {
            foreach (var file in projectFile.VBProject.Modules.Concat(projectFile.VBProject.OtherFiles))
            {
                var uri = new WorkspaceFileUri(file.Uri, workspaceRoot);
                LoadWorkspaceFile(uri, isSourceFile: file is Module, file.IsAutoOpen, projectFile.VBProject.ProjectType);
            }
        }

        private void LoadWorkspaceFile(WorkspaceFileUri uri, bool isSourceFile, bool open = false, ProjectType projectType = ProjectType.VBA)
        {
            var state = _state.ActiveWorkspace!;
            if (state != null && state.WorkspaceRoot != null)
            {
                TryRunAction(() =>
                {
                    var localPath = uri.AbsoluteLocation.LocalPath;
                    var isLoadError = false;
                    var isMissing = !_fileSystem.File.Exists(localPath);
                    var fileVersion = isMissing ? -1 : 1;
                    var content = string.Empty;

                    if (isMissing)
                    {
                        LogWarning($"Missing {(isSourceFile ? "source" : string.Empty)} file: {uri}");
                    }
                    else
                    {
                        try
                        {
                            content = _fileSystem.File.ReadAllText(localPath);
                        }
                        catch (Exception exception)
                        {
                            LogWarning("Could not load file content.", $"File: {uri}");
                            LogException(exception);
                            isLoadError = true;
                        }
                    }

                    DocumentState info;
                    if (isSourceFile)
                    {
                        // project source files are VB source code, always.
                        var language = projectType == ProjectType.VBA ? SupportedLanguage.VBA : SupportedLanguage.VB6;
                        info = new DocumentState(uri, content, isOpened: open && !isMissing)
                        {
                            Language = language,
                            IsMissing = isMissing,
                            IsLoadError = isLoadError
                        };
                    }
                    else
                    {
                        // markdown/text, json, sql, anything else, would be a non-source document file.
                        info = new DocumentState(uri, content, version: 1, open && !isMissing)
                        {
                            IsMissing = isMissing,
                            IsLoadError = isLoadError
                        };
                    }

                    if (state.LoadDocumentState(info))
                    {
                        if (!isMissing)
                        {
                            LogInformation($"Successfully loaded {(isSourceFile ? "source" : string.Empty)} file at {uri}.", $"IsOpened: {info.IsOpened}");
                        }
                        else
                        {
                            LogInformation($"Loaded {(isSourceFile ? "source" : string.Empty)} file at {uri}.", $"IsMissing: {isMissing}; IsLoadError: {isLoadError}");
                        }
                    }
                    else
                    {
                        LogWarning($"{(isSourceFile ? "Source file" : "File")} version {fileVersion} at {uri} was not loaded; a newer version is already cached.'.");
                    }
                }, logPerformance: false);
            }
        }

        public void CloseFile(WorkspaceFileUri uri)
        {
            if (_state.ActiveWorkspace != null)
            {
                _state.ActiveWorkspace.CloseWorkspaceFile(uri, out _);
            }
        }

        public void CloseAllFiles()
        {
            if (_state.ActiveWorkspace != null)
            {
                foreach (var file in _state.ActiveWorkspace.WorkspaceFiles)
                {
                    _state.ActiveWorkspace.CloseWorkspaceFile(file.Uri, out _);
                }
            }
        }

        public void CloseWorkspace()
        {
            var uri = _state.ActiveWorkspace?.WorkspaceRoot ?? throw new InvalidOperationException("WorkspaceStateManager.WorkspaceRoot is unexpectedly null.");

            CloseAllFiles();
            _state.Unload(uri);

            OnWorkspaceClosed(uri);
        }

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            CloseWorkspace();
            Dispose(true);
        }
    }
}

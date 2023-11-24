using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.NewProject;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Rubberduck.Editor
{
    public class WorkspaceService : ServiceBase, IWorkspaceService, IDisposable
    {
        private readonly WorkspaceStateManager _state;
        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileService _projectFile;

        private readonly Dictionary<Uri, IFileSystemWatcher> _watchers = [];
        private readonly Func<ILanguageClient> _lsp;

        public WorkspaceService(ILogger<WorkspaceService> logger, RubberduckSettingsProvider settingsProvider,
            WorkspaceStateManager state, IFileSystem fileSystem, PerformanceRecordAggregator performance,
            IProjectFileService projectFile, Func<ILanguageClient> lsp)
            : base(logger, settingsProvider, performance)
        {
            _state = state;
            _fileSystem = fileSystem;
            _projectFile = projectFile;
            _lsp = lsp;
        }

        public ProjectTemplate ToProjectTemplate() => new()
        {
            Rubberduck = "3.0",
            Name = _state.ProjectName,
            ProjectFile = new ProjectFile
            {
                Rubberduck = "3.0",
                Uri = _state.WorkspaceRoot!,
                VBProject = new Project
                {
                    // todo
                }
            }
        };

        public IFileSystem FileSystem => _fileSystem;

        public async Task<bool> OpenProjectWorkspaceAsync(Uri uri)
        {
            return await Task.Run(() =>
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
                    var version = new Version(projectFile.Rubberduck);
                    if (version > new Version("3.0"))
                    {
                        throw new NotSupportedException("This project was created with a version of Rubberduck greater than the one currently running.");
                    }

                    var sourceRoot = _fileSystem.Path.Combine(root, ProjectFile.SourceRoot);
                    if (!_fileSystem.Directory.Exists(sourceRoot))
                    {
                        throw new DirectoryNotFoundException("Project source root folder ('.src') was not found under the secified workspace URI.");
                    }
                    
                    LoadWorkspaceFiles(sourceRoot, projectFile);
                    _state.WorkspaceRoot = uri;
                    _state.ProjectName = _fileSystem.Path.GetFileName(root);
                    EnableFileSystemWatcher(uri);

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
        }

        public bool IsFileSystemWatcherEnabled(Uri root) => _watchers[root].EnableRaisingEvents;
        public void EnableFileSystemWatcher(Uri root)
        {
            if (!Settings.LanguageClientSettings.WorkspaceSettings.EnableFileSystemWatchers)
            {
                LogInformation("EnableFileSystemWatchers setting is not configured to enable file system watchers. External changes are not monitored.");
                return;
            }

            if (!_watchers.TryGetValue(root, out var watcher))
            {
                watcher = ConfigureWatcher(root.LocalPath);
                _watchers[root] = watcher;
            }

            watcher.EnableRaisingEvents = true;
            LogInformation($"FileSystemWatcher is now active for workspace and subfolders.", $"WorkspaceRoot: {root}");
        }

        public void DisableFileSystemWatcher(Uri root)
        {
            if (_watchers.TryGetValue(root, out var watcher))
            {
                watcher.EnableRaisingEvents = false;
                LogInformation($"FileSystemWatcher is now deactived for workspace and subfolders.", $"WorkspaceRoot: {root}");
            }
            else
            {
                LogWarning($"There is no file system watcher configured for this workspace.", $"WorkspaceRoot: {root}");
            }
        }

        public async Task<bool> SaveWorkspaceFileAsync(Uri uri)
        {
            if (_state.WorkspaceRoot != null && _state.TryGetWorkspaceFile(uri, out var file) && file != null)
            {
                var path = _fileSystem.Path.Combine(_state.WorkspaceRoot.LocalPath, ProjectFile.SourceRoot, file.Uri.LocalPath);
                await _fileSystem.File.WriteAllTextAsync(path, file.Content);
                _state.ClearPreviousVersions(uri);
                return true;
            }

            return false;
        }

        public async Task<bool> SaveWorkspaceFileAsAsync(Uri uri, string path)
        {
            if (_state.WorkspaceRoot != null && _state.TryGetWorkspaceFile(uri, out var file) && file != null)
            {
                // note: saves a copy but only keeps the original URI in the workspace
                await _fileSystem.File.WriteAllTextAsync(path, file.Content);
                return true;
            }

            return false;
        }

        public async Task<bool> SaveAllAsync()
        {
            var tasks = new List<Task>();
            if (_state.WorkspaceRoot != null)
            {
                var srcRoot = _fileSystem.Path.Combine(_state.WorkspaceRoot.LocalPath, ProjectFile.SourceRoot);
                foreach (var file in _state.WorkspaceFiles.Where(e => e.IsOpened))
                {
                    var path = _fileSystem.Path.Combine(srcRoot, file.Uri.LocalPath);
                    tasks.Add(_fileSystem.File.WriteAllTextAsync(path, file.Content));
                    _state.ClearPreviousVersions(file.Uri);
                }
            }

            return await Task.WhenAll(tasks).ContinueWith(t => !t.IsFaulted, TaskScheduler.Current);
        }

        private IFileSystemWatcher ConfigureWatcher(string path)
        {
            var watcher = _fileSystem.FileSystemWatcher.New(path);
            watcher.IncludeSubdirectories = true;
            watcher.Error += OnWatcherError;
            watcher.Created += OnWatcherCreated;
            watcher.Changed += OnWatcherChanged;
            watcher.Deleted += OnWatcherDeleted;
            watcher.Renamed += OnWatcherRenamed;
            return watcher;
        }

        private void OnWatcherRenamed(object sender, RenamedEventArgs e)
        {
            TryRunAction(() =>
            {
                var sourceRoot = e.OldFullPath[..(e.OldFullPath.IndexOf(ProjectFile.SourceRoot) + ProjectFile.SourceRoot.Length)];

                var oldRelativePath = _fileSystem.Path.GetRelativePath(sourceRoot, e.OldFullPath);
                var oldUri = new Uri(oldRelativePath);

                var newRelativePath = _fileSystem.Path.GetRelativePath(sourceRoot, e.FullPath);
                var newUri = new Uri(newRelativePath);

                if (_state.TryGetWorkspaceFile(oldUri, out var workspaceFile) && workspaceFile is not null)
                {
                    if (!_state.RenameWorkspaceFile(oldUri, newUri))
                    {
                        // houston, we have a problem. name collision? validate and notify, unload conflicted file?
                        LogWarning("Rename failed.", $"OldUri: {oldUri}; NewUri: {newUri}");
                    }
                }

                var request = new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(
                        new FileEvent
                        {
                            Uri = oldUri,
                            Type = FileChangeType.Deleted
                        },
                        new FileEvent
                        {
                            Uri = newUri,
                            Type = FileChangeType.Created
                        })
                };

                // NOTE: this is different than the DidRenameFiles mechanism.
                LogTrace("Sending DidChangeWatchedFiles LSP notification...", $"Renamed: {oldUri} -> {newUri}");
                _lsp().Workspace.DidChangeWatchedFiles(request);
            });
        }

        private void OnWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            var sourceRoot = e.FullPath[..(e.FullPath.IndexOf(ProjectFile.SourceRoot) + ProjectFile.SourceRoot.Length)];

            var relativePath = _fileSystem.Path.GetRelativePath(sourceRoot, e.FullPath);
            var uri = new Uri(relativePath);

            _state.UnloadWorkspaceFile(uri);

            var request = new DidChangeWatchedFilesParams
            {
                Changes = new Container<FileEvent>(
                    new FileEvent
                    {
                        Uri = uri,
                        Type = FileChangeType.Deleted
                    })
            };

            // NOTE: this is different than the DidDeleteFiles mechanism.
            LogTrace("Sending DidChangeWatchedFiles LSP notification...", $"Deleted: {uri}");
            _lsp().Workspace.DidChangeWatchedFiles(request);
        }

        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            var sourceRoot = e.FullPath[..(e.FullPath.IndexOf(ProjectFile.SourceRoot) + ProjectFile.SourceRoot.Length)];

            var relativePath = _fileSystem.Path.GetRelativePath(sourceRoot, e.FullPath);
            var uri = new Uri(relativePath);

            _state.UnloadWorkspaceFile(uri);

            var request = new DidChangeWatchedFilesParams
            {
                Changes = new Container<FileEvent>(
                    new FileEvent
                    {
                        Uri = uri,
                        Type = FileChangeType.Changed
                    })
            };

            // NOTE: this is different than the document-level syncing mechanism.
            LogTrace("Sending DidChangeWatchedFiles LSP notification...", $"Changed: {uri}");
            _lsp().Workspace.DidChangeWatchedFiles(request);
        }

        private void OnWatcherCreated(object sender, FileSystemEventArgs e)
        {
            var sourceRoot = e.FullPath[..(e.FullPath.IndexOf(ProjectFile.SourceRoot) + ProjectFile.SourceRoot.Length)];

            var relativePath = _fileSystem.Path.GetRelativePath(sourceRoot, e.FullPath);
            var uri = new Uri(relativePath);

            var request = new DidChangeWatchedFilesParams
            {
                Changes = new Container<FileEvent>(
                    new FileEvent
                    {
                        Uri = uri,
                        Type = FileChangeType.Created
                    })
            };

            // NOTE: this is different than the document-level syncing mechanism.
            LogTrace("Sending DidChangeWatchedFiles LSP notification...", $"Created: {uri}");
            _lsp().Workspace.DidChangeWatchedFiles(request);
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            var exception = e.GetException();
            LogException(exception);

            if (sender is IFileSystemWatcher watcher)
            {
                DisableFileSystemWatcher(new Uri(watcher.Path));
            }
        }


        private void LoadWorkspaceFiles(string sourceRoot, ProjectFile projectFile)
        {
            foreach (var file in projectFile.VBProject.Modules.Concat(projectFile.VBProject.OtherFiles))
            {
                LoadWorkspaceFile(file.Uri, sourceRoot, isSourceFile: file is Module, file.IsAutoOpen);
            }
        }

        private void LoadWorkspaceFile(string uri, string sourceRoot, bool isSourceFile, bool open = false)
        {
            TryRunAction(() =>
            {
                var filePath = _fileSystem.Path.Combine(sourceRoot, uri);

                var isLoadError = false;
                var isMissing = !_fileSystem.File.Exists(filePath);
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
                        content = _fileSystem.File.ReadAllText(filePath);
                    }
                    catch (Exception exception)
                    {
                        LogWarning("Could not load file content.", $"File: {uri}");
                        LogException(exception);
                        isLoadError = true;
                    }
                }

                var info = new WorkspaceFileInfo
                {
                    Uri = new Uri(uri),
                    Content = content,
                    Version = fileVersion,
                    IsSourceFile = isSourceFile,
                    IsOpened = open && !isMissing,
                    IsMissing = isMissing,
                    IsLoadError = isLoadError
                };

                if (_state.LoadWorkspaceFile(info))
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
            });
        }

        public void CloseFile(Uri uri)
        {
            _state.CloseWorkspaceFile(uri, out _);
        }

        public void CloseAllFiles()
        {
            foreach(var file in _state.WorkspaceFiles)
            {
                _state.CloseWorkspaceFile(file.Uri, out _);
            }
        }

        public void CloseWorkspace()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.Error -= OnWatcherError;
                watcher.Created -= OnWatcherCreated;
                watcher.Changed -= OnWatcherChanged;
                watcher.Deleted -= OnWatcherDeleted;
                watcher.Renamed -= OnWatcherRenamed;
                watcher.Dispose();
            }
            _watchers.Clear();
            _state.UnloadWorkspace();
        }
    }
}

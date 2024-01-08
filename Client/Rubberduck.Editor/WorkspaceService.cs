using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.Editor
{
    public class WorkspaceService : ServiceBase, IWorkspaceService, IDisposable
    {
        private readonly IWorkspaceStateManager _state;
        private readonly HashSet<ProjectFile> _projectFiles = new();

        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileService _projectFile;
        private readonly List<Reference> _references = [];

        private readonly Dictionary<Uri, IFileSystemWatcher> _watchers = [];
        private readonly LanguageClientApp _lspClientApp;

        public event EventHandler<WorkspaceServiceEventArgs> WorkspaceOpened = delegate { };
        public event EventHandler<WorkspaceServiceEventArgs> WorkspaceClosed = delegate { };

        public WorkspaceService(ILogger<WorkspaceService> logger, RubberduckSettingsProvider settingsProvider,
            IWorkspaceStateManager state, IFileSystem fileSystem, PerformanceRecordAggregator performance,
            IProjectFileService projectFile, LanguageClientApp lspClientApp)
            : base(logger, settingsProvider, performance)
        {
            _state = state;
            _fileSystem = fileSystem;
            _projectFile = projectFile;
            _lspClientApp = lspClientApp;
        }

        public void OnWorkspaceOpened(Uri uri)
        {
            WorkspaceOpened(this, new(uri));
        }

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

                    var state = _state.AddWorkspace(uri);
                    state.ProjectName = _fileSystem.Path.GetFileName(root);

                    LoadWorkspaceFiles(uri, projectFile);
                    EnableFileSystemWatcher(uri);
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

            if (result)
            {
                if (_lspClientApp.LanguageClient is null)
                {
                    await _lspClientApp.StartupAsync(Settings.LanguageServerSettings.StartupSettings, uri);
                }

                OnWorkspaceOpened(uri);
            }

            return result;
        }

        public bool IsFileSystemWatcherEnabled(Uri root)
        {
            var localPath = root.LocalPath;
            if (localPath.EndsWith(ProjectFile.FileName))
            {
                root = new Uri(localPath[..^(ProjectFile.FileName.Length + 1)]);
            }
            return _watchers.TryGetValue(root, out var value) && value.EnableRaisingEvents;
        }

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

        public async Task<bool> SaveWorkspaceFileAsync(WorkspaceFileUri uri)
        {
            if (_state.ActiveWorkspace?.WorkspaceRoot != null && _state.ActiveWorkspace.TryGetWorkspaceFile(uri, out var file) && file != null)
            {
                var path = _fileSystem.Path.Combine(_state.ActiveWorkspace.WorkspaceRoot.LocalPath, ProjectFile.SourceRoot, file.Uri.LocalPath);
                await _fileSystem.File.WriteAllTextAsync(path, file.Content);
                return true;
            }

            return false;
        }

        public async Task<bool> SaveWorkspaceFileAsAsync(WorkspaceFileUri uri, string path)
        {
            if (_state.ActiveWorkspace?.WorkspaceRoot != null && _state.ActiveWorkspace.TryGetWorkspaceFile(uri, out var file) && file != null)
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
            if (_state.ActiveWorkspace?.WorkspaceRoot != null)
            {
                var srcRoot = _fileSystem.Path.Combine(_state.ActiveWorkspace.WorkspaceRoot.LocalPath, ProjectFile.SourceRoot);
                foreach (var file in _state.ActiveWorkspace.WorkspaceFiles.Where(e => e.IsModified))
                {
                    var path = _fileSystem.Path.Combine(srcRoot, file.Uri.ToString());
                    tasks.Add(_fileSystem.File.WriteAllTextAsync(path, file.Content));

                    file.ResetModifiedState();
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
            var state = _state.ActiveWorkspace;
            if (state != null && state.WorkspaceRoot != null)
            {
                TryRunAction(() =>
                {
                    var oldUri = new WorkspaceFileUri(e.OldFullPath[(state.WorkspaceRoot.LocalPath + $"\\{ProjectFile.SourceRoot}").Length..], state.WorkspaceRoot);
                    var newUri = new WorkspaceFileUri(e.FullPath[(state.WorkspaceRoot.LocalPath + $"\\{ProjectFile.SourceRoot}").Length..], state.WorkspaceRoot);

                    if (state != null && state.TryGetWorkspaceFile(oldUri, out var workspaceFile) && workspaceFile is not null)
                    {
                        if (!state.RenameWorkspaceFile(oldUri, newUri))
                        {
                            // houston, we have a problem. name collision? validate and notify, unload conflicted file?
                            LogWarning("Rename failed.", $"OldUri: {oldUri.AbsoluteLocation}; NewUri: {newUri.AbsoluteLocation}");
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
                    LogTrace("Sending DidChangeWatchedFiles LSP notification...", $"Renamed: {oldUri.AbsoluteLocation} -> {newUri.AbsoluteLocation}");
                    _lspClientApp.LanguageClient?.Workspace.DidChangeWatchedFiles(request);
                });
            }
        }

        private void OnWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            var state = _state.ActiveWorkspace;
            if (state != null && state.WorkspaceRoot != null)
            {
                var uri = new WorkspaceFileUri(e.FullPath[(state.WorkspaceRoot.LocalPath + $"\\{ProjectFile.SourceRoot}").Length..], state.WorkspaceRoot);
                state.UnloadWorkspaceFile(uri);

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
                LogTrace("Sending DidChangeWatchedFiles LSP notification...", $"Deleted: {uri.AbsoluteLocation}");
                _lspClientApp.LanguageClient?.Workspace.DidChangeWatchedFiles(request);
            }
        }

        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            var state = _state.ActiveWorkspace;
            if (state != null && state.WorkspaceRoot != null)
            {
                var uri = new WorkspaceFileUri(e.FullPath[(state.WorkspaceRoot.LocalPath + $"\\{ProjectFile.SourceRoot}").Length..], state.WorkspaceRoot);
                state.UnloadWorkspaceFile(uri);

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
                _lspClientApp.LanguageClient?.Workspace.DidChangeWatchedFiles(request);
            }
        }

        private void OnWatcherCreated(object sender, FileSystemEventArgs e)
        {
            var state = _state.ActiveWorkspace;
            if (state != null && state.WorkspaceRoot != null)
            {
                var relativePath = e.FullPath[(state.WorkspaceRoot.LocalPath + $"\\{ProjectFile.SourceRoot}").Length..];
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
                _lspClientApp.LanguageClient?.Workspace.DidChangeWatchedFiles(request);
            }
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


        private void LoadWorkspaceFiles(Uri workspaceRoot, ProjectFile projectFile)
        {
            foreach (var file in projectFile.VBProject.Modules.Concat(projectFile.VBProject.OtherFiles))
            {
                var uri = new WorkspaceFileUri(file.Uri, workspaceRoot);
                LoadWorkspaceFile(uri, isSourceFile: file is Module, file.IsAutoOpen);
            }
        }

        private void LoadWorkspaceFile(WorkspaceFileUri uri, bool isSourceFile, bool open = false)
        {
            var state = _state.ActiveWorkspace!;
            if (state != null && state.WorkspaceRoot != null)
            {
                TryRunAction(() =>
                {
                    var isLoadError = false;
                    var isMissing = !_fileSystem.File.Exists(uri.AbsoluteLocation.LocalPath);
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
                            content = _fileSystem.File.ReadAllText(uri.AbsoluteLocation.LocalPath);
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
                        Uri = uri,
                        Content = content,
                        IsSourceFile = isSourceFile,
                        IsOpened = open && !isMissing,
                        IsMissing = isMissing,
                        IsLoadError = isLoadError
                    };

                    if (state.LoadWorkspaceFile(info))
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
            CloseWorkspace();
        }
    }
}

using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.Editor;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.UI.Services.Abstract
{
    public class WorkspaceClientService : WorkspaceService
    {
        private readonly LanguageClientApp _lspClientApp;
        private readonly Dictionary<Uri, IFileSystemWatcher> _watchers = [];

        public WorkspaceClientService(ILogger<WorkspaceService> logger, RubberduckSettingsProvider settingsProvider, IWorkspaceStateManager state, IFileSystem fileSystem, PerformanceRecordAggregator performance, IProjectFileService projectFile, LanguageClientApp lspClientApp) 
            : base(logger, settingsProvider, state, fileSystem, performance, projectFile)
        {
            _lspClientApp = lspClientApp;
        }

        public async override Task OnWorkspaceOpenedAsync(Uri uri)
        {
            if (_lspClientApp.LanguageClient is null)
            {
                await _lspClientApp.StartupAsync(Settings.LanguageServerSettings.StartupSettings, uri);
            }

            await base.OnWorkspaceOpenedAsync(uri);
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

        private IFileSystemWatcher ConfigureWatcher(string path)
        {
            var watcher = FileSystem.FileSystemWatcher.New(path);
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
            var state = State.ActiveWorkspace;
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
            var state = State.ActiveWorkspace;
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
            var state = State.ActiveWorkspace;
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
            var state = State.ActiveWorkspace;
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

        protected override void Dispose(bool disposing)
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
        }
    }
}

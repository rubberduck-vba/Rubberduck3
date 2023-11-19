﻿using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.NewProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.Editor
{
    public class WorkspaceService : ServiceBase, IDisposable
    {
        private readonly WorkspaceStateManager _state;
        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileService _projectFile;

        private readonly Dictionary<Uri, IFileSystemWatcher> _watchers = [];
        private readonly ILanguageClientFacade _lsp;

        public WorkspaceService(ILogger logger, RubberduckSettingsProvider settingsProvider,
            WorkspaceStateManager state, IFileSystem fileSystem, 
            IProjectFileService projectFile, ILanguageClientFacade lsp)
            : base(logger, settingsProvider)
        {
            _state = state;
            _fileSystem = fileSystem;
            _projectFile = projectFile;
            _lsp = lsp;
        }

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

                    var sourceRoot = _fileSystem.Path.Combine(root, ProjectFile.SourceRoot);
                    if (!_fileSystem.Directory.Exists(sourceRoot))
                    {
                        throw new DirectoryNotFoundException("Project source root folder ('.src') was not found under the secified workspace URI.");
                    }

                    var projectFile = _projectFile.ReadFile(uri);
                    var version = new Version(projectFile.Rubberduck);
                    if (version > new Version("3.0"))
                    {
                        throw new NotSupportedException("This project was created with a version of Rubberduck greater than the one currently running.");
                    }

                    LoadSourceFiles(sourceRoot, projectFile);
                    LoadNonSourceFiles(sourceRoot, projectFile);
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

        private void EnableFileSystemWatcher(Uri root)
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
                _lsp.Workspace.DidChangeWatchedFiles(request);
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
            _lsp.Workspace.DidChangeWatchedFiles(request);
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
            _lsp.Workspace.DidChangeWatchedFiles(request);
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
            _lsp.Workspace.DidChangeWatchedFiles(request);
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

        private void DisableFileSystemWatcher(Uri root)
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

        private void LoadSourceFiles(string sourceRoot, ProjectFile projectFile)
        {
            foreach (var sourceFile in projectFile.VBProject.Modules)
            {
                LoadWorkspaceFile(sourceFile.Uri, sourceRoot, isSourceFile: true);
            }
        }

        private void LoadNonSourceFiles(string sourceRoot, ProjectFile projectFile)
        {
            foreach (var file in projectFile.VBProject.OtherFiles)
            {
                LoadWorkspaceFile(file, sourceRoot, isSourceFile: false);
            }
        }

        private void LoadWorkspaceFile(Uri uri, string sourceRoot, bool isSourceFile, bool open = false)
        {
            TryRunAction(() =>
            {
                var filePath = _fileSystem.Path.Combine(sourceRoot, uri.LocalPath);

                var isLoadError = false;
                var isMissing = !_fileSystem.File.Exists(filePath);
                var fileVersion = isMissing ? -1 : 1;
                var content = string.Empty;

                if (isMissing)
                {
                    LogWarning($"Missing {(isSourceFile ? "source" : string.Empty)} file: {uri.LocalPath}");
                }
                else
                {
                    try
                    {
                        content = _fileSystem.File.ReadAllText(filePath);
                    }
                    catch (Exception exception)
                    {
                        LogWarning("Could not load file content.", $"File: {uri.LocalPath}");
                        LogException(exception);
                        isLoadError = true;
                    }
                }

                var info = new WorkspaceFileInfo
                {
                    Uri = uri,
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
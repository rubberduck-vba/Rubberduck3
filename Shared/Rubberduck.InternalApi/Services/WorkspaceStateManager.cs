using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Services;

public class WorkspaceStateManager : ServiceBase, IWorkspaceStateManager
{
    private class ProjectStateManager : ServiceBase, IWorkspaceState
    {
        private readonly HashSet<Reference> _references = [];
        private readonly HashSet<Folder> _folders = [];
        private readonly ConcurrentDictionary<Uri, WorkspaceFileInfo> _workspaceFiles = [];

        public ProjectStateManager(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
            : base(logger, settingsProvider, performance)
        {
        }

        public Uri? WorkspaceRoot { get; set; }
        public string ProjectName { get; set; } = "Project1";

        public IEnumerable<Folder> Folders => _folders;
        public void AddFolder(Folder folder) => _folders.Add(folder);

        public void RemoveFolder(Folder folder) => _folders.Remove(folder);

        public IEnumerable<Reference> References => _references;

        public void AddHostLibraryReference(Reference reference)
        {
            reference.IsUnremovable = true;
            _references.Add(reference);
        }

        public void AddReference(Reference reference)
        {
            _references.Add(reference);
        }

        public void RemoveReference(Reference reference)
        {
            if (!reference.IsUnremovable)
            {
                _references.Remove(reference);
            }
        }

        public void SwapReferences(ref Reference first, ref Reference second)
        {
            if (first.IsUnremovable || second.IsUnremovable)
            {
                // built-in references (stdlib, hostlib) never move.
                return;
            }

            (first, second) = (second, first);
        }

        public IEnumerable<WorkspaceFileInfo> WorkspaceFiles
        {
            get
            {
                foreach (var file in _workspaceFiles)
                {
                    yield return file.Value;
                }
            }
        }

        public bool LoadWorkspaceFile(WorkspaceFileInfo file)
        {
            if (!_workspaceFiles.TryGetValue(file.Uri, out var existingCache) || file.Content != existingCache.Content)
            {
                _workspaceFiles[file.Uri] = file;
                return true;
            }

            return false;
        }

        public void UnloadAllFiles()
        {
            foreach (var file in _workspaceFiles.Values)
            {
                file.IsOpened = false;
            }
            _workspaceFiles.Clear();
        }

        public bool TryGetWorkspaceFile(WorkspaceFileUri uri, out WorkspaceFileInfo? fileInfo) => _workspaceFiles.TryGetValue(uri, out fileInfo);

        public bool CloseWorkspaceFile(WorkspaceFileUri uri, out WorkspaceFileInfo? fileInfo)
        {
            if (_workspaceFiles.TryGetValue(uri, out fileInfo))
            {
                if (!fileInfo.IsOpened)
                {
                    fileInfo.IsOpened = false;
                    return true;
                }
            }

            fileInfo = default;
            return false;
        }

        public bool RenameWorkspaceFile(WorkspaceFileUri oldUri, WorkspaceFileUri newUri)
        {
            if (_workspaceFiles.TryGetValue(newUri, out _))
            {
                // new URI already exists... TODO check for a name collision
                return false;
            }

            if (_workspaceFiles.TryGetValue(oldUri, out var oldCache))
            {
                _workspaceFiles[newUri] = oldCache with { Uri = newUri };
            }

            return false;
        }

        public bool UnloadWorkspaceFile(WorkspaceFileUri uri)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache))
            {
                cache.IsOpened = false;
                return _workspaceFiles.TryRemove(uri, out _);
            }

            return false;
        }
    }

    public WorkspaceStateManager(ILogger<WorkspaceStateManager> logger,
        RubberduckSettingsProvider settings, PerformanceRecordAggregator performance)
        : base(logger, settings, performance)
    {
    }

    private Dictionary<Uri, IWorkspaceState> _workspaces = [];
    public IWorkspaceState GetWorkspace(Uri workspaceRoot)
    {
        if (!_workspaces.Any())
        {
            throw new InvalidOperationException("Workspace data is empty.");
        }

        if (!_workspaces.TryGetValue(workspaceRoot, out var value))
        {
            LogWarning("Workspace URI was not found.", $"{workspaceRoot}\n{string.Join("\n*", _workspaces.Keys.Select(key => key.ToString()))}");
            throw new KeyNotFoundException("Workspace URI was not found.");
        }

        return value;
    }

    public IEnumerable<IWorkspaceState> Workspaces => _workspaces.Values;
    public IWorkspaceState? ActiveWorkspace { get; set; }

    public IWorkspaceState AddWorkspace(Uri workspaceRoot)
    {
        var state = new ProjectStateManager(_logger, SettingsProvider, _performance)
        {
            WorkspaceRoot = workspaceRoot
        };
        _workspaces[workspaceRoot] = state;
        ActiveWorkspace = state;
        return state;
    }

    public void Unload(Uri workspaceRoot)
    {
        if (_workspaces.TryGetValue(workspaceRoot, out var state))
        {
            state.UnloadAllFiles();
            _workspaces.Remove(workspaceRoot);
        }
    }
}

using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Services;

public class WorkspaceStateManager : ServiceBase, IWorkspaceStateManager
{
    private class ProjectStateManager : ServiceBase, IWorkspaceState
    {
        private readonly HashSet<Reference> _references = [];
        private readonly HashSet<Folder> _folders = [];
        private readonly DocumentContentStore _store;

        public ProjectStateManager(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance,
            DocumentContentStore store)
            : base(logger, settingsProvider, performance)
        {
            _store = store;
            ExecutionContext = new VBExecutionContext(logger, settingsProvider, performance);
        }

        public VBExecutionContext ExecutionContext { get; }

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

        public IEnumerable<DocumentState> WorkspaceFiles
        {
            get
            {
                foreach (var file in _store.Enumerate())
                {
                    yield return file;
                }
            }
        }

        public bool LoadWorkspaceFile(DocumentState file)
        {
            _store.AddOrUpdate(file.Uri, file);
            return true;
        }

        public void UnloadAllFiles()
        {
            var files = _store.Enumerate().ToArray();
            foreach (var file in files)
            {
                _store.TryRemove(file.Uri);
            }
        }

        public bool TryGetWorkspaceFile(WorkspaceFileUri uri, out DocumentState? state) => _store.TryGetDocument(uri, out state);

        public bool CloseWorkspaceFile(WorkspaceFileUri uri, out DocumentState? state)
        {
            if (_store.TryGetDocument(uri, out state))
            {
                if (state!.IsOpened)
                {
                    state = state.WithOpened(false);
                    _store.AddOrUpdate(uri, state);
                    return true;
                }
            }

            state = default;
            return false;
        }

        public bool RenameWorkspaceFile(WorkspaceFileUri oldUri, WorkspaceFileUri newUri)
        {
            if (_store.TryGetDocument(newUri, out _))
            {
                // new URI already exists... TODO check for a name collision
                return false;
            }

            if (_store.TryGetDocument(oldUri, out var oldCache))
            {
                _store.AddOrUpdate(newUri, oldCache! with { Uri = newUri });
            }

            return false;
        }

        public bool UnloadWorkspaceFile(WorkspaceFileUri uri)
        {
            if (_store.TryGetDocument(uri, out _))
            {
                return _store.TryRemove(uri);
            }

            return false;
        }

        public bool SaveWorkspaceFile(WorkspaceFileUri uri)
        {
            if (_store.TryGetDocument(uri, out var file))
            {
                _store.AddOrUpdate(uri, file!.WithResetVersion());
                return true;
            }
            return false;
        }
    }

    private readonly DocumentContentStore _store;

    public WorkspaceStateManager(ILogger<WorkspaceStateManager> logger, RubberduckSettingsProvider settings, PerformanceRecordAggregator performance,
        DocumentContentStore store)
        : base(logger, settings, performance)
    {
        _store = store;
    }

    private Dictionary<Uri, IWorkspaceState> _workspaces = [];
    public IWorkspaceState GetWorkspace(WorkspaceUri workspaceRoot)
    {
        if (!_workspaces.Any())
        {
            throw new InvalidOperationException("Workspace data is empty.");
        }

        if (!_workspaces.TryGetValue(workspaceRoot.WorkspaceRoot, out var value))
        {
            LogWarning("Workspace URI was not found.", $"{workspaceRoot.WorkspaceRoot}\n{string.Join("\n*", _workspaces.Keys.Select(key => key.ToString()))}");
            throw new KeyNotFoundException("Workspace URI was not found.");
        }

        return value;
    }

    public IEnumerable<IWorkspaceState> Workspaces => _workspaces.Values;
    public IWorkspaceState? ActiveWorkspace { get; set; }

    public IWorkspaceState AddWorkspace(Uri workspaceRoot)
    {
        var state = new ProjectStateManager(Logger, SettingsProvider, Performance, _store)
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

using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Editor
{
    public class WorkspaceStateManager : ServiceBase, IWorkspaceStateManager
    {
        private readonly HashSet<Reference> _references = [];
        private readonly HashSet<Folder> _folders = [];
        private readonly ConcurrentDictionary<Uri, ConcurrentQueue<WorkspaceFileInfo>> _workspaceFiles = [];

        public WorkspaceStateManager(ILogger<WorkspaceStateManager> logger, 
            RubberduckSettingsProvider settings, PerformanceRecordAggregator performance)
            : base(logger, settings, performance)
        {
        }

        public Uri? WorkspaceRoot { get; set; }
        public string ProjectName { get; set; } = "Project1";

        public bool TryGetWorkspaceFile(Uri uri, out WorkspaceFileInfo? fileInfo)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache))
            {
                return cache.TryPeek(out fileInfo);
            }

            fileInfo = default;
            return false;
        }

        public bool TryGetWorkspaceFile(Uri uri, int version, out WorkspaceFileInfo? fileInfo)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache))
            {
                fileInfo = cache.SingleOrDefault(e => e.Version == version);
                return fileInfo != null;
            }

            fileInfo = default;
            return false;
        }

        public IEnumerable<Folder> Folders => _folders;
        public void AddFolder(Folder folder)
        {
            _folders.Add(folder);
        }

        public void RemoveFolder(Folder folder)
        {
            _folders.Remove(folder);
        }

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
            _references.Remove(reference);
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
                    if (file.Value.TryPeek(out var cache))
                    {
                        yield return cache;
                    }
                }
            }
        }

        public bool CloseWorkspaceFile(Uri uri, out WorkspaceFileInfo? fileInfo)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache)
                && cache.TryPeek(out fileInfo))
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

        public void ClearPreviousVersions(Uri uri)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache) && cache.TryDequeue(out var current))
            {
                cache.Clear();
                var file = current with { Version = 1 };
                cache.Enqueue(file);
            }
        }

        public bool LoadWorkspaceFile(WorkspaceFileInfo file, int cacheCapacity = 3)
        {
            if (!_workspaceFiles.TryGetValue(file.Uri, out var existingCache))
            {
                var queue = new ConcurrentQueue<WorkspaceFileInfo>();
                queue.Enqueue(file);
                _workspaceFiles[file.Uri] = queue;
                return true;
            }
            else if (existingCache.TryPeek(out var existing))
            {
                if (file.Version > existing.Version)
                {
                    if (file.Content != existing.Content)
                    {
                        var cached = _workspaceFiles[file.Uri];
                        while (cached.Count >= cacheCapacity)
                        {
                            cached.TryDequeue(out _);
                        }
                        cached.Enqueue(file);
                        return true;
                    }
                    // else: same content, skip this version.
                }
                // else: old version, skip.
            }
            else
            {
                // invalid but recoverable state: no content in cache but URI is legitimate
                existingCache.Enqueue(file);
            }
            return false;
        }

        public bool RenameWorkspaceFile(Uri oldUri, Uri newUri)
        {
            if (_workspaceFiles.TryGetValue(newUri, out var existingCache))
            {
                // new URI already exists... TODO check for a name collision
                return false;
            }

            if (_workspaceFiles.TryGetValue(oldUri, out var oldCache) && oldCache.TryPeek(out var oldFileInfo))
            {
                // keep the old cache key around but point it to the updated uri as a newer version
                var version = oldFileInfo.Version + 1;
                var newFileInfo = oldFileInfo with { Uri = newUri, Version = version };
                oldCache.Enqueue(newFileInfo);

                var newCache = new ConcurrentQueue<WorkspaceFileInfo>([newFileInfo]);
                _workspaceFiles[newUri] = newCache;
            }

            return false;
        }

        public bool UnloadWorkspaceFile(Uri uri)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache))
            {
                cache.Clear();
                return _workspaceFiles.TryRemove(uri, out _);
            }

            return false;
        }

        public void UnloadWorkspace()
        {
            _workspaceFiles.Clear();
            WorkspaceRoot = null;
            // should this force a GC? does it matter?
        }
    }
}

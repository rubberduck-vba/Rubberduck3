using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Manages the state and content of each file in a workspace.
    /// </summary>
    public class WorkspaceStateManager
    {
        private readonly ConcurrentDictionary<Uri, ConcurrentQueue<WorkspaceFileInfo>> _workspaceFiles = [];
        private const int CacheCapacity = 3; // TODO make this a setting / NOTE: this is not the editor's undo stack!

        /// <summary>
        /// Marks the file at the specified URI as opened in the editor.
        /// </summary>
        /// <param name="uri">The URI referring to the file to open in the editor.</param>
        /// <returns>The latest available version of the file.</returns>
        public WorkspaceFileInfo? OpenWorkspaceFile(Uri uri)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache) 
                && cache.TryPeek(out var fileInfo))
            {
                fileInfo.IsOpened = true;
                return fileInfo;
            }

            return default;
        }

        /// <summary>
        /// Attempts to retrieve the specified file.
        /// </summary>
        /// <param name="uri">The URI referring to the file to retrieve.</param>
        /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
        /// <returns><c>true</c> if the specified version was found.</returns>
        public bool TryGetWorkspaceFile(Uri uri, out WorkspaceFileInfo? fileInfo)
        {
            if (_workspaceFiles.TryGetValue(uri, out var cache))
            {
                return cache.TryPeek(out fileInfo);
            }

            fileInfo = default;
            return false;
        }

        /// <summary>
        /// Attempts to retrieve the specified version of the specified file.
        /// </summary>
        /// <param name="uri">The URI referring to the file to retrieve.</param>
        /// <param name="version">The specific version number to retrieve.</param>
        /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
        /// <returns><c>true</c> if the specified version was found.</returns>
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

        /// <summary>
        /// Marks the file at the specified URI as closed in the editor.
        /// </summary>
        /// <param name="uri">The URI referring to the file to mark as closed.</param>
        /// <param name="fileInfo">Holds a non-null reference if the file was found.</param>
        /// <returns><c>true</c> if the workspace file was correctly found and marked as closed.</returns>
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

        /// <summary>
        /// Loads the specified file into the workspace.
        /// </summary>
        /// <param name="file">The file (including its content) to be added.</param>
        /// <param name="cacheCapacity">The number of versions held in cache.</param>
        /// <returns><c>true</c> if the file was successfully added to the workspace.</returns>
        /// <remarks>This method will overwrite a cached URI for a newer version if the content is different.</remarks>
        public bool LoadWorkspaceFile(WorkspaceFileInfo file, int cacheCapacity = CacheCapacity)
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
            // should this force a GC? does it matter?
        }
    }
}

using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.Services
{
    public class WorkspaceServiceEventArgs : EventArgs
    {
        public WorkspaceServiceEventArgs(Uri uri)
        {
            Uri = uri;
        }

        public Uri Uri { get; }
    }

    public interface IAppWorkspacesService : IDisposable
    {
        event EventHandler<WorkspaceServiceEventArgs> WorkspaceOpened;
        event EventHandler<WorkspaceServiceEventArgs> WorkspaceClosed;

        IFileSystem FileSystem { get; }

        IAppWorkspacesStateManager Workspaces { get; }
        /// <summary>
        /// Gets the project file for all loaded workspaces.
        /// </summary>
        IEnumerable<ProjectFile> ProjectFiles { get; }

        Task<bool> OpenProjectWorkspaceAsync(Uri uri);

        //bool IsFileSystemWatcherEnabled(Uri root);
        //void EnableFileSystemWatcher(Uri root);
        //void DisableFileSystemWatcher(Uri root);

        Task<bool> SaveWorkspaceFileAsync(WorkspaceFileUri uri);
        Task<bool> SaveWorkspaceFileAsAsync(WorkspaceFileUri uri, string path);
        Task<bool> SaveAllAsync();

        void CloseFile(WorkspaceFileUri uri);
        void CloseAllFiles();
        void CloseWorkspace();
    }
}

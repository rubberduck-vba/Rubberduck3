using Rubberduck.UI.NewProject;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.UI.Services.Abstract
{
    public interface IWorkspaceService : IDisposable
    {
        IFileSystem FileSystem { get; }
        ProjectTemplate ToProjectTemplate();

        Task<bool> OpenProjectWorkspaceAsync(Uri uri);
        bool IsFileSystemWatcherEnabled(Uri root);
        void EnableFileSystemWatcher(Uri root);
        void DisableFileSystemWatcher(Uri root);

        Task<bool> SaveWorkspaceFileAsync(Uri uri);
        Task<bool> SaveWorkspaceFileAsAsync(Uri uri, string path);
        Task<bool> SaveAllAsync();
        void CloseFile(Uri uri);
        void CloseAllFiles();
        void CloseWorkspace();
    }
}

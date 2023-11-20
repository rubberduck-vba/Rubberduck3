using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Rubberduck.UI.Services.Abstract
{
    public interface IWorkspaceService : IDisposable
    {
        IFileSystem FileSystem { get; }

        Task<bool> OpenProjectWorkspaceAsync(Uri uri);
        bool IsFileSystemWatcherEnabled(Uri root);
        void EnableFileSystemWatcher(Uri root);
        void DisableFileSystemWatcher(Uri root);
    }
}

using Rubberduck.InternalApi.Common;
using System;
using System.IO.Abstractions;

namespace Rubberduck.Client.LSP
{
    /// <summary>
    /// Provides readonly access to host document properties and
    /// IDirectoryInfo objects for key Workspace directories.
    /// </summary>
    public interface IWorkspaceInfo
    {
        /// <summary>
        /// Read only Absolute path to the host document of the Workspace.
        /// </summary>
        string HostDocumentPath { get; }

        /// <summary>
        /// The host document file name
        /// </summary>
        string HostDocumentName { get; }

        /// <summary>
        /// Returns an IDirectoryInfo object related to the
        /// Rubberduck '.rd' directory.
        /// </summary>
        IDirectoryInfo DotRdInfo { get; }

        /// <summary>
        /// Returns an IDirectoryInfo object related to the 'Working' workspace folder.
        /// The 'Working' folder contains unsaved file versions base on
        /// the last exit from the RD3 editor.
        /// </summary>
        /// <remarks>
        /// When the host document is saved, the 'Working' repository is cleared.
        /// </remarks>
        IDirectoryInfo WorkingRepoInfo { get; }

        /// <summary>
        /// Returns an IDirectoryInfo object related to the 'Saved' workspace folder.
        /// The 'Saved' folder contains module exports from
        /// from the last user-initiated `Save` of the host document.  
        /// </summary>
        IDirectoryInfo SavedRepoInfo { get; }
    }

    /// <summary>
    /// Provides readonly access to host document properties and
    /// IDirectoryInfo objects for Workspace directories.
    /// </summary>
    /// <remarks>
    /// Initializes objects based on the host document path. It 
    /// does not perform IO operation on the file system.
    /// </remarks>
    public struct WorkspaceInfo : IWorkspaceInfo
    {
        private const string wsFoldersName = "WorkspaceFolders";

        private readonly string _hostDocumentPath;
        private readonly IFileSystem _fileSystem;

        private readonly Func<string, IDirectoryInfo> _dirInfoFactoryFunc;

        /// <summary>
        /// Creates a WorkspaceInfo struct based on the absolute path to a host document.
        /// Other parameters should be left 'null' unless injecting a fake 
        /// object for testing.
        /// </summary>
        /// <param name="hostDocumentPath">Absolute path to the host document</param>
        /// <param name="fileSystem">Optional: should be omitted except for injecting test fakes</param>
        /// <param name="dirInfoFunc">Optional: should be omitted except for injecting test fakes</param>
        /// <exception cref="ArgumentNullException"></exception>
        public WorkspaceInfo(string hostDocumentPath, 
            IFileSystem fileSystem = null, 
            Func<string, IDirectoryInfo> dirInfoFunc = null)
        {
            if (string.IsNullOrEmpty(hostDocumentPath))
            {
                throw new ArgumentNullException(nameof(hostDocumentPath));
            }

            _hostDocumentPath = hostDocumentPath;
            
            _dirInfoFactoryFunc = dirInfoFunc
                ?? ((s) => new FileSystem().DirectoryInfo.New(s));

            _fileSystem = fileSystem ?? new FileSystem();

            var hostDocumentFileInfo = _fileSystem.FileInfo.New(_hostDocumentPath);

            HostDocumentName = hostDocumentFileInfo.Name;

            var dotRdPath = string.Join("\\", 
                _fileSystem.Directory.GetParent(_hostDocumentPath).FullName, ".rd");
            DotRdInfo = _dirInfoFactoryFunc(dotRdPath);

            var workspaceName = hostDocumentFileInfo.Name.Replace(
                $"{hostDocumentFileInfo.Extension}", 
                $"_{hostDocumentFileInfo.Extension.Substring(1)}");

            var workspaceDirectoryPath = string.Join("\\", DotRdInfo, workspaceName);
            WorkspaceDirectoryInfo = _dirInfoFactoryFunc(workspaceDirectoryPath);

            var savedRepoPath = string.Join("\\",
                workspaceDirectoryPath, wsFoldersName, "saved");
            SavedRepoInfo = _dirInfoFactoryFunc(savedRepoPath);

            var workingRepoPath = string.Join("\\",
                workspaceDirectoryPath, wsFoldersName, "working");
            WorkingRepoInfo = _dirInfoFactoryFunc(workingRepoPath);
        }

        /// <inheritdoc/>
        public string HostDocumentPath => _hostDocumentPath;

        public string HostDocumentName { get; private set; }

        /// <inheritdoc/>
        public IDirectoryInfo DotRdInfo { get; private set; }

        /// <inheritdoc/>
        public IDirectoryInfo WorkspaceDirectoryInfo { get; private set; }

        /// <inheritdoc/>
        public IDirectoryInfo WorkingRepoInfo { get; private set; }

        /// <inheritdoc/>
        public IDirectoryInfo SavedRepoInfo { get; private set; }
    }
}

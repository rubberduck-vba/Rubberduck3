﻿using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Services
{
    public interface IWorkspaceState
    {
        Uri? WorkspaceRoot { get; set; }
        string ProjectName { get; set; }
        IEnumerable<WorkspaceFileInfo> WorkspaceFiles { get; }

        /// <summary>
        /// Attempts to retrieve the specified file.
        /// </summary>
        /// <param name="uri">The URI referring to the file to retrieve.</param>
        /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
        /// <returns><c>true</c> if the specified version was found.</returns>
        bool TryGetWorkspaceFile(WorkspaceFileUri uri, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Marks the file at the specified URI as closed in the editor.
        /// </summary>
        /// <param name="uri">The URI referring to the file to mark as closed.</param>
        /// <param name="fileInfo">Holds a non-null reference if the file was found.</param>
        /// <returns><c>true</c> if the workspace file was correctly found and marked as closed.</returns>
        bool CloseWorkspaceFile(WorkspaceFileUri uri, out WorkspaceFileInfo? fileInfo);
        /// <summary>
        /// Loads the specified file into the workspace.
        /// </summary>
        /// <param name="file">The file (including its content) to be added.</param>
        /// <returns><c>true</c> if the file was successfully added to the workspace.</returns>
        /// <remarks>This method will overwrite a cached URI if URI matches an existing file.</remarks>
        bool LoadWorkspaceFile(WorkspaceFileInfo file);
        /// <summary>
        /// Renames the specified workspace URI.
        /// </summary>
        /// <param name="oldUri">The old URI.</param>
        /// <param name="newUri">The new URI.</param>
        /// <returns><c>true</c> if the rename was successful.</returns>
        bool RenameWorkspaceFile(WorkspaceFileUri oldUri, WorkspaceFileUri newUri);
        /// <summary>
        /// Unloads the specified workspace URI.
        /// </summary>
        /// <param name="uri">The file URI to unload.</param>
        /// <returns><c>true</c> if the file was successfully unloaded.</returns>
        bool UnloadWorkspaceFile(WorkspaceFileUri uri);
        void UnloadAllFiles();
    }
}

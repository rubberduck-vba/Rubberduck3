using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Services;

public interface IWorkspaceState
{
    event EventHandler<WorkspaceFileUriEventArgs> WorkspaceFileStateChanged;
    WorkspaceUri? WorkspaceRoot { get; set; }
    string ProjectName { get; set; }
    void PublishDiagnostics(int? version, DocumentUri uri, IEnumerable<Diagnostic> diagnostics);
    IEnumerable<DocumentState> WorkspaceFiles { get; }
    IEnumerable<Reference> References { get; }
    VBExecutionContext ExecutionContext { get; }

    void AddReference(Reference reference);
    void RemoveReference(Reference reference);

    bool SaveWorkspaceFile(WorkspaceFileUri uri);
    /// <summary>
    /// Attempts to retrieve the specified file.
    /// </summary>
    /// <param name="uri">The URI referring to the file to retrieve.</param>
    /// <param name="fileInfo">The retrieved <c>WorkspaceFileInfo</c>, if found.</param>
    /// <returns><c>true</c> if the specified version was found.</returns>
    bool TryGetWorkspaceFile(WorkspaceFileUri uri, out DocumentState? fileInfo);
    /// <summary>
    /// Marks the file at the specified URI as closed in the editor.
    /// </summary>
    /// <param name="uri">The URI referring to the file to mark as closed.</param>
    /// <param name="fileInfo">Holds a non-null reference if the file was found.</param>
    /// <returns><c>true</c> if the workspace file was correctly found and marked as closed.</returns>
    bool CloseWorkspaceFile(WorkspaceFileUri uri, out DocumentState? fileInfo);
    /// <summary>
    /// Loads the specified file into the workspace.
    /// </summary>
    /// <param name="file">The file (including its content) to be added.</param>
    /// <returns><c>true</c> if the file was successfully added (or overwritten) to the workspace.</returns>
    /// <remarks>This method will overwrite a cached URI if URI matches an existing file.</remarks>
    bool LoadDocumentState(DocumentState file);
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

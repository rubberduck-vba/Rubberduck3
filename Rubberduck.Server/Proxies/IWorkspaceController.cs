using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Configuration;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    public interface IWorkspaceController
    {
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.Language.Diagnostics)]
        Task<WorkspaceFullDocumentDiagnosticReport> DiagnosticAsync(WorkspaceDiagnosticsParams parameters);

        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.Language.Symbols)]
        Task<WorkspaceSymbol[]> SymbolAsync(WorkspaceSymbolParams parameters);

        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.Server.Configuration)]
        Task<object[]> Configuration(ConfigurationParams parameters);

        /// <summary>
        /// Signals a client-side change of configuration settings.
        /// </summary>
        event EventHandler<DidChangeConfigurationParams> DidChangeConfiguration;
        /// <summary>
        /// Fetches the current list of open workspace folders.
        /// </summary>
        /// <returns>
        /// <c>null</c> if only a single file is open, or an empty array if a workspace is open but no folders are configured.
        /// </returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.Folders)]
        Task<WorkspaceFolder[]> WorkspaceFoldersAsync();

        /// <summary>
        /// Informs the server of a change in workspace folder configurations.
        /// </summary>
        event EventHandler<DidChangeWorkspaceFoldersParams> DidChangeWorkspaceFolders;

        /// <summary>
        /// Initiates the creation of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.WillCreate)]
        Task<WorkspaceEdit> WillCreateFilesAsync(CreateFilesParams parameters);

        /// <summary>
        /// Signals the client-side creation of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.DidCreate)]
        Task<WorkspaceEdit> DidCreateFilesAsync(CreateFilesParams parameters);

        /// <summary>
        /// Initiates the rename of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.WillRename)]
        Task<WorkspaceEdit> WillRenameFilesAsync(RenameFilesParams parameters);

        /// <summary>
        /// Signals the client-side renaming of files/folders.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.DidRename)]
        Task<WorkspaceEdit> DidRenameFilesAsync(RenameFilesParams parameters);

        /// <summary>
        /// Initiates the deletion of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.WillDelete)]
        Task<WorkspaceEdit> WillDeleteFilesAsync(DeleteFilesParams parameters);

        /// <summary>
        /// Signals the client-side deletion of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.Workspace.File.DidDelete)]
        Task<WorkspaceEdit> DidDeleteFilesAsync(DeleteFilesParams parameters);
    }
}

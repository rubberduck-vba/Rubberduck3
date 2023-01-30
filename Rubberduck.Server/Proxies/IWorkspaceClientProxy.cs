using System.Threading.Tasks;
using Rubberduck.RPC.Platform.Metadata;
using System;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;

namespace Rubberduck.Server.LSP.Proxies
{
    public interface IWorkspaceClientProxy
    {
        [LspCompliant(JsonRpcMethods.ClientProxyRequests.LSP.ApplyWorkspaceEdit)]
        Task<ApplyWorkspaceEditResult> ApplyEditAsync(ApplyWorkspaceEditParams parameters);

        event EventHandler RefreshCodeLens;
        event EventHandler RefreshDiagnostics;
        event EventHandler RefreshInlayHints;
        event EventHandler RefreshSemanticTokens;

        /// <summary>
        /// Signals a client-side FileSystemWatcher event.
        /// </summary>
        event EventHandler<DidChangeWatchedFilesParams> DidChangeWatchedFiles;

        /// <summary>
        /// Executes a command on the server.
        /// </summary>
        event EventHandler<ExecuteCommandParams> ExecuteCommand;
    }
}

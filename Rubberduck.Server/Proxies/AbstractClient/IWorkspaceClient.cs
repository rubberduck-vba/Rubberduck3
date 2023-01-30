using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using StreamJsonRpc;
using System.Threading.Tasks;

namespace Rubberduck.Server.LSP.Controllers.AbstractClient
{
    /// <summary>
    /// Workspace-level requests sent from a server to a client.
    /// </summary>
    public interface IWorkspaceClient
    {
        /// <summary>
        /// A request that prompts a client to refresh shown code lenses.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.RefreshCodeLens)]
        Task RefreshCodeLensAsync();
        
        /// <summary>
        /// A request that prompts a client to refresh semantic tokens.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.RefreshSemanticTokens)]
        Task RefreshSemanticTokensAsync();

        /// <summary>
        /// A request that prompts a client to refresh inlay hints.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.RefreshInlayHints)]
        Task RefreshInlayHintsAsync();

        /// <summary>
        /// A request that prompts a client to refresh diagnostics.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.RefreshDiagnostics)]
        Task RefreshDiagnosticsAsync();

        /// <summary>
        /// A request that prompts a client to apply a set of document edits.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.LSP.ApplyWorkspaceEdit)]
        Task<ApplyWorkspaceEditResult> ApplyEditAsync(ApplyWorkspaceEditParams parameters);
    }
}

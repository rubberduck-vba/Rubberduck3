using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
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
        [JsonRpcMethod(JsonRpcMethods.RefreshCodeLens)]
        Task RefreshCodeLens();
        
        /// <summary>
        /// A request that prompts a client to refresh semantic tokens.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.RefreshSemanticTokens)]
        Task RefreshSemanticTokens();

        /// <summary>
        /// A request that prompts a client to refresh inlay hints.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.RefreshInlayHints)]
        Task RefreshInlayHints();

        /// <summary>
        /// A request that prompts a client to refresh diagnostics.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.RefreshDiagnostics)]
        Task RefreshDiagnostics();

        /// <summary>
        /// A request that prompts a client to apply a set of document edits.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ApplyWorkspaceEdit)]
        Task<ApplyWorkspaceEditResult> ApplyEdit(ApplyWorkspaceEditParams parameters);
    }
}

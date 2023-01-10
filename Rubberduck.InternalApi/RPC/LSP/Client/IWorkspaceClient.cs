using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.LSP.Client
{
    /// <summary>
    /// Workspace-level requests sent from a server to a client.
    /// </summary>
    [ServiceContract]
    public interface IWorkspaceClient
    {
        /// <summary>
        /// A request that prompts a client to refresh shown code lenses.
        /// </summary>
        [OperationContract(Name = "workspace/codeLens/refresh")]
        Task RefreshCodeLens();
        
        /// <summary>
        /// A request that prompts a client to refresh semantic tokens.
        /// </summary>
        [OperationContract(Name = "workspace/semanticTokens/refresh")]
        Task RefreshSemanticTokens();

        /// <summary>
        /// A request that prompts a client to refresh inlay hints.
        /// </summary>
        [OperationContract(Name = "workspace/inlayHint/refresh")]
        Task RefreshInlayHints();

        /// <summary>
        /// A request that prompts a client to refresh diagnostics.
        /// </summary>
        [OperationContract(Name = "workspace/diagnostic/refresh")]
        Task RefreshDiagnostics();

        /// <summary>
        /// A request that prompts a client to apply a set of document edits.
        /// </summary>
        [OperationContract(Name = "workspace/applyEdit")]
        Task<ApplyWorkspaceEditResult> ApplyEdit(ApplyWorkspaceEditParams parameters);
    }
}

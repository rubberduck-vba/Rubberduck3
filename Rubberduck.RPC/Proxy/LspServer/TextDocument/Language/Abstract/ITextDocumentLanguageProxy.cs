using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using StreamJsonRpc;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Abstract
{
    public interface ITextDocumentLanguageProxy
    {
        /// <summary>
        /// The <strong>PrepareTypeHierarchy</strong> request is sent from the client to the server to return a type hierarchy for the language element of given text document positions.
        /// </summary>
        /// <remarks>
        /// The type hierarchy requests are executed in two steps:
        /// <list type="number">
        /// <item>first a type hierarchy item is prepared for the given text document position.</item>
        /// <item>for a type hierarchy item the <em>supertype</em> or <em>subtype</em> type hierarchy items are resolved.</item>
        /// </list>
        /// </remarks>
        /// <returns>An array of <c>TypeHierarchyItem</c> items.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.PrepareTypeHierarchy), LspCompliant]
        Task<TypeHierarchyItem[]> TypeHierarchyPrepareAsync(TypeHierarchyPrepareParams parameters);


        /// <summary>
        /// Resolves the supertypes (base classes) for a given type hierarchy item.
        /// </summary>
        /// <param name="parameters">The type hierarchy item to resolve</param>
        /// <returns>An array of <c>TypeHierarchyItem</c> objects. May be <c>null</c> or empty if the server couldn’t infer a valid type from the position</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.SuperTypes), LspCompliant]
        Task<TypeHierarchyItem[]> TypeHierarchySuperTypesAsync(TypeHierarchySupertypesParams parameters);

        /// <summary>
        /// Resolves the subtypes (derived classes) for a given type hierarchy item.
        /// </summary>
        /// <param name="parameters">The type hierarchy item to resolve</param>
        /// <returns>An array of <c>TypeHierarchyItem</c> objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.SubTypes), LspCompliant]
        Task<TypeHierarchyItem[]> TypeHierarchySubTypesAsync(TypeHierarchySubtypesParams parameters);

        /// <summary>
        /// The call hierarchy request is sent from the client to the server to return a call hierarchy for the language element of given text document positions.
        /// </summary>
        /// <remarks>
        /// The call hierarchy requests are executed in two steps:
        /// <list type="number">
        /// <item>first a call hierarchy item is resolved for the given text document position</item>
        /// <item>for a call hierarchy item the <em>incoming</em> or <em>outgoing</em> call hierarchy items are resolved.</item>
        /// </list>
        /// </remarks>
        /// <returns>An array of <c>CallHierarchyItem</c> items.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.PrepareCallHierarchy), LspCompliant]
        Task<CallHierarchyItem[]> CallHierarchyPrepareAsync(CallHierarchyPrepareParams parameters);

        /// <summary>
        /// The request is sent from the client to the server to resolve incoming calls for a given call hierarchy item.
        /// </summary>
        /// <remarks>
        /// The request doesn’t define its own client and server capabilities. It is only issued if a server registers for the <c>textDocument/prepareCallHierarchy</c> request.
        /// </remarks>
        /// <param name="parameters">The call hierarchy item to resolve.</param>
        /// <returns>An array of <c>CallHierarchyIncomingCall</c> items.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.CallHierarchyIncoming), LspCompliant]
        Task<CallHierarchyIncomingCall[]> IncomingCallsAsync(CallHierarchyIncomingCallsParams parameters);

        /// <summary>
        /// The <strong>OutgoingCalls</strong> request is sent from the client to the server to resolve outgoing calls for a given call hierarchy item.
        /// </summary>
        /// <param name="parameters">The call hierarchy item to resolve.</param>
        /// <returns>A <c>CallHierarchyOutgoingCall[]</c> object.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.CallHierarchyOutgoing), LspCompliant]
        Task<CallHierarchyOutgoingCall[]> OutgoingCallsAsync(CallHierarchyOutgoingCallsParams parameters);

        /// <summary>
        /// The <strong>CodeAction</strong> request is sent from the client to the server to compute commands for a given text document and range.
        /// </summary>
        /// <remarks>
        /// These commands are typically code fixes to either fix problems or to beautify/refactor code.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.CodeAction), LspCompliant]
        Task<CodeAction[]> CodeActionAsync(CodeActionParams parameters);

        /// <summary>
        /// The text document <strong>Disagnostic</strong> request is sent from the client to the server to ask the server to compute the diagnostics for a given document.
        /// </summary>
        /// <remarks>
        /// As with other pull requests, the server is asked to compute the diagnostics for the <em>currently synced</em> version of the document.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.Diagnostics), LspCompliant]
        Task<FullDocumentDiagnosticReport> DiagnosticAsync(DocumentDiagnosticsParams parameters);

        /// <summary>
        /// The <strong>DocumentSymbol</strong> request is sent from the client to the server. The returned result is a hierarchy of symbols found in a given text document.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.Symbols), LspCompliant]
        Task<DocumentSymbol[]> DocumentSymbolAsync(DocumentSymbolParams parameters);

        /// <summary>
        /// The textDocument/moniker request is sent from the client to the server to get the symbol monikers for a given text document position.
        /// </summary>
        /// <remarks>
        /// An array of <c>Moniker</c> types is returned as response to indicate possible monikers at the given location. 
        /// If no monikers can be calculated, an empty array (or <c>null</c>) should be returned.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.Monikers), LspCompliant]
        Task<Moniker[]> MonikerAsync(MonikerParams parameters);

        /// <summary>
        /// The <strong>PrepareRename</strong> request is sent from the client to the server to setup and test the validity of a rename operation at a given location.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.PrepareRename), LspCompliant]
        Task<Range> PrepareRenameAsync(PrepareRenameParams parameters);

        /// <summary>
        /// The <strong>Rename</strong> request is sent from the client to the server to ask the server to compute a workspace change so that the client can perform a workspace-wide rename of a symbol.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.Rename), LspCompliant]
        Task<WorkspaceEdit> RenameAsync(RenameParams parameters);

        /// <summary>
        /// The <strong>SemanticTokens</strong> request is sent from the client to the server to resolve semantic tokens for a given file.
        /// </summary>
        /// <remarks>
        /// Semantic tokens are used to add additional color information to a file that depends on language-specific symbol information.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.SemanticTokens), LspCompliant]
        Task<SemanticTokens> SemanticTokensAsync(SemanticTokensParams parameters);

        /// <summary>
        /// The <strong>SemanticTokensDelta</strong> request is sent from the client to the server to resolve semantic tokens for a given file.
        /// </summary>
        /// <remarks>
        /// Semantic tokens are used to add additional color information to a file that depends on language-specific symbol information.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.SemanticTokensDelta), LspCompliant]
        Task<SemanticTokensDelta> SemanticTokensDeltaAsync(SemanticTokensDeltaParams parameters);

        /// <summary>
        /// The <strong>SemanticTokensFull</strong> request is sent from the client to the server to resolve semantic tokens for a given file.
        /// </summary>
        /// <remarks>
        /// Semantic tokens are used to add additional color information to a file that depends on language-specific symbol information.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Language.SemanticTokensFull), LspCompliant]
        Task<SemanticTokens> SemanticTokensFullAsync(SemanticTokensParams parameters);
    }
}
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using StreamJsonRpc;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Abstract
{
    public interface ITextDocumentNavigationProxy
    {
        /// <summary>
        /// 'Go To Declaration' request resolves the declaration location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.GoToDeclarations), LspCompliant]
        Task<LocationLink[]> GoToDeclarationAsync(DeclarationParams parameters);

        /// <summary>
        /// 'Go To Definition' request resolves the definition location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.GoToDefinition), LspCompliant]
        Task<LocationLink[]> GoToDefinitionAsync(DefinitionParams parameters);

        /// <summary>
        /// 'Got To Type Definition' request resolves the type definition location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.GoToTypeDefinition), LspCompliant]
        Task<LocationLink[]> GoToTypeDefinitionAsync(TypeDefinitionParams parameters);

        /// <summary>
        /// 'Go To Implementation' request resolves the implementation location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.GoToImplementation), LspCompliant]
        Task<LocationLink[]> GoToImplementationAsync(ImplementationParams parameters);

        /// <summary>
        /// 'Find all References' request resolves the reference locations of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.FindReferences), LspCompliant]
        Task<LocationLink[]> FindReferencesAsync(ReferencesParams parameters);

        /// <summary>
        /// Gets the location of all links in a document.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.DocumentLinks), LspCompliant]
        Task<DocumentLink[]> DocumentLinkAsync(DocumentLinkParams parameters);

        /// <summary>
        /// Resolves properties for a specified <c>DocumentLink</c>.
        /// </summary>
        /// <param name="parameters">The <c>DocumentLink</c> to resolve.</param>
        /// <returns>The resolved document link.</returns>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.ResolveDocumentLink), LspCompliant]
        Task<DocumentLink> ResolveAsync(DocumentLink parameters);

        /// <summary>
        /// The <strong>CodeLens</strong> request is sent from the client to the server to compute code lenses for a given text document.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.LSP.TextDocument.Navigation.CodeLens), LspCompliant]
        Task<CodeLens[]> CodeLensAsync(CodeLensParams parameters);
    }
}

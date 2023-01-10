using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class TextDocumentController
    {
        /// <summary>
        /// A notification sent to the server from the client to signal newly opened text documents.
        /// </summary>
        /// <remarks>
        /// Server ability to fulfill a request is independent of whether a text document is open or closed.
        /// </remarks>
        /// <param name="parameters">The document that was opened.</param>
        [OperationContract(Name = "textDocument/didOpen")]
        public async Task DidOpen(DidOpenTextDocumentParams parameters)
        {
            
        }

        /// <summary>
        /// A notification that signals client-side changes to a text document.
        /// </summary>
        /// <param name="parameters">The document that changed.</param>
        [OperationContract(Name = "textDocument/didChange")]
        public async Task DidChange(DidChangeTextDocumentParams parameters)
        {

        }

        /// <summary>
        /// A notification sent before the document is actually saved.
        /// </summary>
        /// <param name="parameters">The document that will be saved.</param>
        /// <returns></returns>
        [OperationContract(Name = "textDocument/willSave")]
        public async Task WillSave(WillSaveTextDocumentParams parameters)
        {

        }

        /// <summary>
        /// Gets an array of TextEdits that will be applied to the document before it is saved.
        /// </summary>
        /// <param name="parameters">The document that will be saved.</param>
        /// <returns></returns>
        [OperationContract(Name = "textDocument/willSaveWaitUntil")]
        public async Task<TextEdit[]> WillSaveWaitUntil(WillSaveTextDocumentParams parameters)
        {
            return null;
        }

        /// <summary>
        /// A notification sent to the server when the client saves a document.
        /// </summary>
        [OperationContract(Name = "textDocument/didSave")]
        public async Task DidSave(DidSaveTextDocumentParams parameters)
        {

        }

        /// <summary>
        /// A notification sent to the server when the client closes a document.
        /// </summary>
        /// <param name="parameters">The document that was closed.</param>
        [OperationContract(Name = "textDocument/didClose")]
        public async Task DidClose(DidCloseTextDocumentParams parameters)
        {

        }

        /// <summary>
        /// 'Go To Declaration' request resolves the declaration location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [OperationContract(Name = "textDocument/declaration")]
        public async Task<LocationLink[]> GoToDeclaration(DeclarationParams parameters)
        {
            return null;
        }

        /// <summary>
        /// 'Go To Definition' request resolves the definition location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [OperationContract(Name = "textDocument/definition")]
        public async Task<LocationLink[]> GoToDefinition(DefinitionParams parameters)
        {
            return null;
        }

        /// <summary>
        /// 'Got To Type Definition' request resolves the type definition location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [OperationContract(Name = "textDocument/typeDefinition")]
        public async Task<LocationLink[]> GoToTypeDefinition(TypeDefinitionParams parameters)
        {
            return null;
        }

        /// <summary>
        /// 'Go To Implementation' request resolves the implementation location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [OperationContract(Name = "textDocument/implementation")]
        public async Task<LocationLink[]> GoToImplementation(ImplementationParams parameters)
        {
            return null;
        }

        /// <summary>
        /// 'Find all References' request resolves the reference locations of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [OperationContract(Name = "textDocument/references")]
        public async Task<LocationLink[]> FindReferences(ReferencesParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/prepareCallHierarchy")]
        public async Task<CallHierarchyItem[]> CallHierarchyPrepare(CallHierarchyPrepareParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/prepareTypeHierarchy")]
        public async Task<TypeHierarchyItem[]> TypeHierarchyPrepare(TypeHierarchyPrepareParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/documentHighlight")]
        public async Task<DocumentHighlight[]> DocumentHighlights(DocumentHighlightParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Gets the location of all links in a document.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract(Name = "textDocument/documentLink")]
        public async Task<DocumentLink[]> DocumentLink(DocumentLinkParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/hover")]
        public async Task<Hover> Hover(HoverParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/codeLens")]
        public async Task<CodeLens[]> CodeLens(CodeLensParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/foldingRange")]
        public async Task<FoldingRange[]> FoldingRange(FoldingRangeParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/selectionRange")]
        public async Task<SelectionRange[]> SelectionRange(SelectionRangeParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/documentSymbol")]
        public async Task<DocumentSymbol[]> DocumentSymbol(DocumentSymbolParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/semanticTokens")]
        public async Task<SemanticTokens> SemanticTokens(SemanticTokensParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/semanticTokens/full")]
        public async Task<SemanticTokens> SemanticTokensFull(SemanticTokensParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/semanticTokens/full/delta")]
        public async Task<SemanticTokensDelta> SemanticTokensDelta(SemanticTokensDeltaParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/inlayHint")]
        public async Task<InlayHint[]> InlayHint(InlayHintParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/moniker")]
        public async Task<Moniker[]> Moniker(MonikerParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/completion")]
        public async Task<CompletionList> Completion(CompletionParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/diagnostic")]
        public async Task<FullDocumentDiagnosticReport> Diagnostic(DocumentDiagnosticsParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/signatureHelp")]
        public async Task<SignatureHelp> SignatureHelp(SignatureHelpParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/codeAction")]
        public async Task<CodeAction> CodeAction(CodeActionParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/documentColor")]
        public async Task<ColorInformation[]> DocumentColor(DocumentColorParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/colorPresentation")]
        public async Task<ColorPresentation[]> ColorPresentation(ColorPresentationParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/formatting")]
        public async Task<TextEdit[]> Formatting(DocumentFormattingParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/rangeFormatting")]
        public async Task<TextEdit[]> RangeFormatting(RangeFormattingParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/onTypeFormatting")]
        public async Task<TextEdit[]> OnTypeFormatting(DocumentOnTypeFormattingParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/rename")]
        public async Task<WorkspaceEdit> Rename(RenameParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/prepareRename")]
        public async Task<Range> PrepareRename(PrepareRenameParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "textDocument/linkedEditingRanges")]
        public async Task<LinkedEditingRanges> LinkedEditingRange(LinkedEditingRangeParams parameters)
        {
            return null;
        }
    }
}

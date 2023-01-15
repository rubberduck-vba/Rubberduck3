using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class TextDocumentController : JsonRpcClient
    {
        public TextDocumentController(WebSocket socket) : base(socket)
        {
        }

        /// <summary>
        /// A notification sent to the server from the client to signal newly opened text documents.
        /// </summary>
        /// <remarks>
        /// Server ability to fulfill a request is independent of whether a text document is open or closed.
        /// </remarks>
        /// <param name="parameters">The document that was opened.</param>
        [JsonRpcMethod(JsonRpcMethods.DidOpen)]
        public async Task DidOpen(DidOpenTextDocumentParams parameters)
        {
            await Task.Run(() =>
            { 
                var request = CreateRequest(JsonRpcMethods.DidOpen, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// A notification that signals client-side changes to a text document.
        /// </summary>
        /// <param name="parameters">The document that changed.</param>
        [JsonRpcMethod(JsonRpcMethods.DidChange)]
        public async Task DidChange(DidChangeTextDocumentParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DidChange, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// A notification sent before the document is actually saved.
        /// </summary>
        /// <param name="parameters">The document that will be saved.</param>
        /// <returns></returns>
        [JsonRpcMethod(JsonRpcMethods.WillSave)]
        public async Task WillSave(WillSaveTextDocumentParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WillSave, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// Gets an array of TextEdits that will be applied to the document before it is saved.
        /// </summary>
        /// <param name="parameters">The document that will be saved.</param>
        /// <returns></returns>
        [JsonRpcMethod(JsonRpcMethods.WillSaveUntil)]
        public async Task<TextEdit[]> WillSaveWaitUntil(WillSaveTextDocumentParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WillSaveUntil, parameters);
                var response = Request<TextEdit[]>(request);

                return response;
            });
        }

        /// <summary>
        /// A notification sent to the server when the client saves a document.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.DidSave)]
        public async Task DidSave(DidSaveTextDocumentParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DidSave, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// A notification sent to the server when the client closes a document.
        /// </summary>
        /// <param name="parameters">The document that was closed.</param>
        [JsonRpcMethod(JsonRpcMethods.DidClose)]
        public async Task DidClose(DidCloseTextDocumentParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DidClose, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// 'Go To Declaration' request resolves the declaration location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.GoToDeclarations)]
        public async Task<LocationLink[]> GoToDeclaration(DeclarationParams parameters)
        {
            return await Task.Run(() =>
            { 
                var request = CreateRequest(JsonRpcMethods.GoToDeclarations, parameters);
                var response = Request<LocationLink[]>(request);

                return response;
            });
        }

        /// <summary>
        /// 'Go To Definition' request resolves the definition location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.GoToDefinition)]
        public async Task<LocationLink[]> GoToDefinition(DefinitionParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.GoToDefinition, parameters);
                var response = Request<LocationLink[]>(request);

                return response;
            });
        }

        /// <summary>
        /// 'Got To Type Definition' request resolves the type definition location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.GoToTypeDefinition)]
        public async Task<LocationLink[]> GoToTypeDefinition(TypeDefinitionParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.GoToTypeDefinition, parameters);
                var response = Request<LocationLink[]>(request);
                
                return response;
            });
        }

        /// <summary>
        /// 'Go To Implementation' request resolves the implementation location of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.GoToImplementation)]
        public async Task<LocationLink[]> GoToImplementation(ImplementationParams parameters)
        {
            return await Task.Run(() =>
            { 
                var request = CreateRequest(JsonRpcMethods.GoToImplementation, parameters);
                var response = Request<LocationLink[]>(request);

                return response;
            });
        }

        /// <summary>
        /// 'Find all References' request resolves the reference locations of a symbol at a given text document position.
        /// </summary>
        /// <param name="parameters">The symbol to resolve.</param>
        /// <returns>An array of LocationLink objects.</returns>
        [JsonRpcMethod(JsonRpcMethods.FindReferences)]
        public async Task<LocationLink[]> FindReferences(ReferencesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.FindReferences, parameters);
                var response = Request<LocationLink[]>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.PrepareCallHierarchy)]
        public async Task<CallHierarchyItem[]> CallHierarchyPrepare(CallHierarchyPrepareParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.PrepareCallHierarchy, parameters);
                var response = Request<CallHierarchyItem[]>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.PrepareTypeHierarchy)]
        public async Task<TypeHierarchyItem[]> TypeHierarchyPrepare(TypeHierarchyPrepareParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.PrepareTypeHierarchy, parameters);
                var response = Request<TypeHierarchyItem[]>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.DocumentHighlights)]
        public async Task<DocumentHighlight[]> DocumentHighlights(DocumentHighlightParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentHighlights, parameters);
                var response = Request<DocumentHighlight[]>(request);

                return response;
            });
        }

        /// <summary>
        /// Gets the location of all links in a document.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [JsonRpcMethod(JsonRpcMethods.DocumentLinks)]
        public async Task<DocumentLink[]> DocumentLink(DocumentLinkParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentLinks, parameters);
                var response = Request<DocumentLink[]>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.DocumentHover)]
        public async Task<Hover> Hover(HoverParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentHover, parameters);
                var response = Request<Hover>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.DocumentCodeLens)]
        public async Task<CodeLens[]> CodeLens(CodeLensParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentCodeLens, parameters);
                var response = Request<CodeLens[]>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.DocumentFoldingRanges)]
        public async Task<FoldingRange[]> FoldingRange(FoldingRangeParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentFoldingRanges, parameters);
                var response = Request<FoldingRange[]>(request);

                return response;
            });
        }

        public async Task<SelectionRange[]> SelectionRange(SelectionRangeParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentSelectionRange, parameters);
                var response = Request<SelectionRange[]>(request);

                return response;
            });
        }

        public async Task<DocumentSymbol[]> DocumentSymbol(DocumentSymbolParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentSymbols, parameters);
                var response = Request<DocumentSymbol[]>(request);

                return response;
            });
        }

        public async Task<SemanticTokens> SemanticTokens(SemanticTokensParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentSemanticTokens, parameters);
                var response = Request<SemanticTokens>(request);

                return response;
            });
        }

        public async Task<SemanticTokens> SemanticTokensFull(SemanticTokensParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentSemanticTokensFull, parameters);
                var response = Request<SemanticTokens>(request);

                return response;
            });
        }

        public async Task<SemanticTokensDelta> SemanticTokensDelta(SemanticTokensDeltaParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentSemanticTokensDelta, parameters);
                var response = Request<SemanticTokensDelta>(request);

                return response;
            });
        }

        public async Task<InlayHint[]> InlayHint(InlayHintParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentInlayHints, parameters);
                var response = Request<InlayHint[]>(request);

                return response;
            });
        }

        public async Task<Moniker[]> Moniker(MonikerParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentMonikers, parameters);
                var response = Request<Moniker[]>(request);

                return response;
            });
        }

        public async Task<CompletionList> Completion(CompletionParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Completion, parameters);
                var response = Request<CompletionList>(request);

                return response;
            });
        }

        public async Task<FullDocumentDiagnosticReport> Diagnostic(DocumentDiagnosticsParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.DocumentDiagnostics, parameters);
                var response = Request<FullDocumentDiagnosticReport>(request);

                return response;
            });
        }

        public async Task<SignatureHelp> SignatureHelp(SignatureHelpParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.SignatureHelp, parameters);
                var response = Request<SignatureHelp>(request);

                return response;
            });
        }

        public async Task<CodeAction> CodeAction(CodeActionParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.CodeAction, parameters);
                var response = Request<CodeAction>(request);

                return response;
            });
        }

        public async Task<ColorInformation[]> DocumentColor(DocumentColorParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Color, parameters);
                var response = Request<ColorInformation[]>(request);

                return response;
            });
        }

        public async Task<ColorPresentation[]> ColorPresentation(ColorPresentationParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ColorPresentation, parameters);
                var response = Request<ColorPresentation[]>(request);

                return response;
            });
        }

        public async Task<TextEdit[]> Formatting(DocumentFormattingParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Formatting, parameters);
                var response = Request<TextEdit[]>(request);

                return response;
            });
        }

        public async Task<TextEdit[]> RangeFormatting(RangeFormattingParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.RangeFormatting, parameters);
                var response = Request<TextEdit[]>(request);

                return response;
            });
        }

        public async Task<TextEdit[]> OnTypeFormatting(DocumentOnTypeFormattingParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.OnTypeFormatting, parameters);
                var response = Request<TextEdit[]>(request);

                return response;
            });
        }

        public async Task<WorkspaceEdit> Rename(RenameParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Rename, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        public async Task<Range> PrepareRename(PrepareRenameParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.PrepareRename, parameters);
                var response = Request<Range>(request);

                return response;
            });
        }

        public async Task<LinkedEditingRanges> LinkedEditingRange(LinkedEditingRangeParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.LinkedEditingRanges, parameters)
                var response = Request<LinkedEditingRanges>(request);

                return response;
            });
        }
    }
}

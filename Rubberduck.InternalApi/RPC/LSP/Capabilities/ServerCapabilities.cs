using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class ServerCapabilities
    {
        [JsonPropertyName("positionEncoding")]
        public string PositionEncodingKind { get; set; } = Constants.PositionEncodingKind.UTF16;

        [JsonPropertyName("textDocumentSync")]
        public TextDocumentSyncOptions TextDocumentSync { get; set; }

        //[JsonPropertyName("notebookDocumentSyncOptions")]
        //public object NotebookDocumentSyncOptions { get; set; } // LSP method not implemented

        [JsonPropertyName("completionProvider")]
        public CompletionOptions CompletionProvider { get; set; }

        [JsonPropertyName("hoverProvider")]
        public HoverOptions HoverProvider { get; set; }

        [JsonPropertyName("signatureHelpProvider")]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }

        [JsonPropertyName("declarationProvider")]
        public DeclarationOptions DeclarationProvider { get; set; }

        [JsonPropertyName("definitionProvider")]
        public DefinitionOptions DefinitionProvider { get; set; }

        [JsonPropertyName("typeDefinitionProvider")]
        public TypeDefinitionOptions TypeDefinitionProvider { get; set; }

        [JsonPropertyName("implementationProvider")]
        public ImplementationOptions ImplementationProvider { get; set; }

        [JsonPropertyName("referenceProvider")]
        public ReferenceOptions ReferenceProvider { get; set; }

        [JsonPropertyName("documentHighlightProvider")]
        public DocumentHighlightOptions DocumentHighlightProvider { get; set; }

        [JsonPropertyName("documentSymbolProvider")]
        public DocumentSymbolProvider DocumentSymbolProvider { get; set; }

        [JsonPropertyName("codeActionProvider")]
        public CodeActionOptions CodeActionProvider { get; set; }

        [JsonPropertyName("codeLensProvider")]
        public CodeLensOptions CodeLensProvider { get; set; }

        [JsonPropertyName("documentLinkProvider")]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }

        [JsonPropertyName("colorProvider")]
        public DocumentColorOptions ColorProvider { get; set; }

        [JsonPropertyName("documentFormattingProvider")]
        public DocumentFormattingOptions DocumentFormattingProvider { get; set; }

        [JsonPropertyName("documentRangeFormattingProvider")]
        public DocumentRangeFormattingOptions DocumentRangeFormattingProvider { get; set; }

        [JsonPropertyName("documentOnTypeFormattingProvider")]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }

        [JsonPropertyName("renameProvider")]
        public RenameOptions RenameProvider { get; set; }

        [JsonPropertyName("foldingRangeProvider")]
        public FoldingRangeOptions FoldingRangeProvider { get; set; }

        [JsonPropertyName("executeCommandProvider")]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }

        [JsonPropertyName("selectionRangeProvider")]
        public SelectionRangeOptions SelectionRangeProvider { get; set; }

        [JsonPropertyName("linkedEditingRangeProvider")]
        public LinkedEditingRangeOptions LinkedEditingRangeProvider { get; set; }

        [JsonPropertyName("callHierarchyProvider")]
        public CallHierarchyOptions CallHierarchyProvider { get; set; }

        [JsonPropertyName("semanticTokensProvider")]
        public SemanticTokensOptions SemanticTokensProvider { get; set; }

        [JsonPropertyName("monikerProvider")]
        public MonikerOptions MonikerProvider { get; set; }

        [JsonPropertyName("typeHierarchyProvider")]
        public TypeHierarchyOptions TypeHierarchyProvider { get; set; }

        [JsonPropertyName("inlineValueProvider")]
        public InlineValueOptions InlineValueProvider { get; set; }

        [JsonPropertyName("inlayHintProvider")]
        public InlayHintOptions InlayHintProvider { get; set; }

        [JsonPropertyName("diagnosticProvider")]
        public DiagnosticOptions DiagnosticProvider { get; set; }

        [JsonPropertyName("workspaceSymbolProvider")]
        public WorkspaceSymbolOptions WorkspaceSymbolProvider { get; set; }

        [JsonPropertyName("workspace")]
        public WorkspaceServerCapabilities Workspace { get; set; }


        [JsonPropertyName("experimental")]
        public LSPAny Experimental { get; set; }
    }
}

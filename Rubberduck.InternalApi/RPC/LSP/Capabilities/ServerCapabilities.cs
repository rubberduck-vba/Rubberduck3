using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "serverCapabilities")]
    public class ServerCapabilities
    {
        [ProtoMember(1, Name = "positionEncoding")]
        public string PositionEncodingKind { get; set; } = Constants.PositionEncodingKind.UTF16;

        [ProtoMember(2, Name = "textDocumentSync")]
        public TextDocumentSyncOptions TextDocumentSync { get; set; }

        [ProtoMember(3, Name = "notebookDocumentSyncOptions")]
        public object NotebookDocumentSyncOptions { get; set; } // not implemented

        [ProtoMember(4, Name = "completionProvider")]
        public CompletionOptions CompletionProvider { get; set; }

        [ProtoMember(5, Name = "hoverProvider")]
        public HoverOptions HoverProvider { get; set; }

        [ProtoMember(6, Name = "signatureHelpProvider")]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }

        [ProtoMember(7, Name = "declarationProvider")]
        public DeclarationOptions DeclarationProvider { get; set; }

        [ProtoMember(8, Name = "definitionProvider")]
        public DefinitionOptions DefinitionProvider { get; set; }

        [ProtoMember(9, Name = "typeDefinitionProvider")]
        public TypeDefinitionOptions TypeDefinitionProvider { get; set; }

        [ProtoMember(10, Name = "implementationProvider")]
        public ImplementationOptions ImplementationProvider { get; set; }

        [ProtoMember(11, Name = "referenceProvider")]
        public ReferenceOptions ReferenceProvider { get; set; }

        [ProtoMember(12, Name = "documentHighlightProvider")]
        public DocumentHighlightOptions DocumentHighlightProvider { get; set; }

        [ProtoMember(13, Name = "documentSymbolProvider")]
        public DocumentSymbolProvider DocumentSymbolProvider { get; set; }

        [ProtoMember(14, Name = "codeActionProvider")]
        public CodeActionOptions CodeActionProvider { get; set; }

        [ProtoMember(15, Name = "codeLensProvider")]
        public CodeLensOptions CodeLensProvider { get; set; }

        [ProtoMember(16, Name = "documentLinkProvider")]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }

        [ProtoMember(17, Name = "colorProvider")]
        public DocumentColorOptions ColorProvider { get; set; }

        [ProtoMember(18, Name = "documentFormattingProvider")]
        public DocumentFormattingOptions DocumentFormattingProvider { get; set; }

        [ProtoMember(19, Name = "documentRangeFormattingProvider")]
        public DocumentRangeFormattingOptions DocumentRangeFormattingProvider { get; set; }

        [ProtoMember(20, Name = "documentOnTypeFormattingProvider")]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }

        [ProtoMember(21, Name = "renameProvider")]
        public RenameOptions RenameProvider { get; set; }

        [ProtoMember(22, Name = "foldingRangeProvider")]
        public FoldingRangeOptions FoldingRangeProvider { get; set; }

        [ProtoMember(23, Name = "executeCommandProvider")]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }

        [ProtoMember(24, Name = "selectionRangeProvider")]
        public SelectionRangeOptions SelectionRangeProvider { get; set; }

        [ProtoMember(25, Name = "linkedEditingRangeProvider")]
        public LinkedEditingRangeOptions LinkedEditingRangeProvider { get; set; }

        [ProtoMember(26, Name = "callHierarchyProvider")]
        public CallHierarchyOptions CallHierarchyProvider { get; set; }

        [ProtoMember(27, Name = "semanticTokensProvider")]
        public SemanticTokensOptions SemanticTokensProvider { get; set; }

        [ProtoMember(28, Name = "monikerProvider")]
        public MonikerOptions MonikerProvider { get; set; }

        [ProtoMember(29, Name = "typeHierarchyProvider")]
        public TypeHierarchyOptions TypeHierarchyProvider { get; set; }

        [ProtoMember(30, Name = "inlineValueProvider")]
        public InlineValueOptions InlineValueProvider { get; set; }

        [ProtoMember(31, Name = "inlayHintProvider")]
        public InlayHintOptions InlayHintProvider { get; set; }

        [ProtoMember(32, Name = "diagnosticProvider")]
        public DiagnosticOptions DiagnosticProvider { get; set; }

        [ProtoMember(33, Name = "workspaceSymbolProvider")]
        public WorkspaceSymbolOptions WorkspaceSymbolProvider { get; set; }

        [ProtoMember(34, Name = "workspace")]
        public WorkspaceServerCapabilities Workspace { get; set; }


        [ProtoMember(35, Name = "experimental")]
        public LSPAny Experimental { get; set; }
    }
}

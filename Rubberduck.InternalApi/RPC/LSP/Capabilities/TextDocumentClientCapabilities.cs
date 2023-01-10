using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "textDocumentClientCapabilities")]
    public class TextDocumentClientCapabilities
    {
        [ProtoMember(1, Name = "synchronization")]
        public TextDocumentSyncClientCapabilities Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/completion' request.
        /// </summary>
        [ProtoMember(2, Name = "completion")]
        public CompletionClientCapabilities Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/hover' request.
        /// </summary>
        [ProtoMember(3, Name = "hover")]
        public HoverClientCapabilities Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/signatureHelp' request.
        /// </summary>
        [ProtoMember(4, Name = "signatureHelp")]
        public SignatureHelpClientCapabilities SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/declaration' request.
        /// </summary>
        [ProtoMember(5, Name = "declaration")]
        public DeclarationClientCapabilities Declaration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/definition' request.
        /// </summary>
        [ProtoMember(6, Name = "definition")]
        public DefinitionClientCapabilities Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/typeDefinition' request.
        /// </summary>
        [ProtoMember(7, Name = "typeDefinition")]
        public TypeDefinitionClientCapabilities TypeDefinition { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/implementation' request.
        /// </summary>
        [ProtoMember(8, Name = "implementation")]
        public ImplementationClientCapabilities Implementation { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/references' request.
        /// </summary>
        [ProtoMember(9, Name = "references")]
        public ReferenceClientCapabilities References { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentHighlight' request.
        /// </summary>
        [ProtoMember(10, Name = "documentHighlight")]
        public DocumentHighlightClientCapabilities DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentSymbol' request.
        /// </summary>
        [ProtoMember(11, Name = "documentSymbol")]
        public DocumentSymbolClientCapabilities DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/codeAction' request.
        /// </summary>
        [ProtoMember(12, Name = "codeAction")]
        public CodeActionClientCapabilities CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/codeLens' request.
        /// </summary>
        [ProtoMember(13, Name = "codeLens")]
        public CodeLensClientCapabilities CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentLink' request.
        /// </summary>
        [ProtoMember(14, Name = "documentLink")]
        public DocumentLinkClientCapabilities DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentColor' and 'textDocument/colorPresentation' requests.
        /// </summary>
        [ProtoMember(15, Name = "colorProvider")]
        public DocumentColorClientCapabilities ColorProvider { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/formatting' request.
        /// </summary>
        [ProtoMember(16, Name = "formatting")]
        public DocumentFormattingClientCapabilities Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/rangeFormatting' request.
        /// </summary>
        [ProtoMember(17, Name = "rangeFormatting")]
        public DocumentRangeFormattingClientCapabilities RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/onTypeFormatting' request.
        /// </summary>
        [ProtoMember(18, Name = "onTypeFormatting")]
        public DocumentOnTypeFormattingClientCapabilities OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/rename' request.
        /// </summary>
        [ProtoMember(19, Name = "rename")]
        public RenameClientCapabilities Rename { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/publishDiagnostics' notification.
        /// </summary>
        [ProtoMember(20, Name = "publishDiagnostics")]
        public PublishDiagnosticsClientCapabilities PublishDiagnostics { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/foldingRange' request.
        /// </summary>
        [ProtoMember(21, Name = "foldingRange")]
        public FoldingRangeClientCapabilities FoldingRange { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/selectionRange' request.
        /// </summary>
        [ProtoMember(22, Name = "selectionRange")]
        public SelectionRangeClientCapabilities SelectionRange { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/linkedEditingRange' request.
        /// </summary>
        [ProtoMember(23, Name = "linkedEditingRange")]
        public LinkedEditingRangeClientCapabilities LinkedEditingRange { get; set; }

        /// <summary>
        /// Capabilities specific to the various call hierarchy requests.
        /// </summary>
        [ProtoMember(24, Name = "callHierarchy")]
        public CallHierarchyClientCapabilities CallHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the various semantic token requests.
        /// </summary>
        [ProtoMember(25, Name = "semanticToken")]
        public SemanticTokenClientCapabilities SemanticToken { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/moniker' request.
        /// </summary>
        [ProtoMember(26, Name = "moniker")]
        public MonikerClientCapabilities Moniker { get; set; }

        /// <summary>
        /// Capabilities specific to the various type hierarchy requests.
        /// </summary>
        [ProtoMember(27, Name = "typeHierarchy")]
        public TypeHierarchyClientCapabilities TypeHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/inlineValue' request.
        /// </summary>
        [ProtoMember(28, Name = "inlineValue")]
        public InlineValueClientCapabilities InlineValue { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/inlayHint' request.
        /// </summary>
        [ProtoMember(29, Name = "inlayHint")]
        public InlayHintClientCapabilities InlayHint { get; set; }

        /// <summary>
        /// Capabilities specific to the diagnostic pull model.
        /// </summary>
        [ProtoMember(30, Name = "diagnostic")]
        public DiagnosticClientCapabilities Diagnostic { get; set; }
    }
}

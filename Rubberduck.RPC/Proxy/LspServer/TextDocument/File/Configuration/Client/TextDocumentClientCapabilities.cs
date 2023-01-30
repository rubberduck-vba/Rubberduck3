using Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration.Client;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration.Client;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Configuration.Client;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Configuration.Client
{
    public class TextDocumentClientCapabilities
    {
        [JsonPropertyName("synchronization")]
        public TextDocumentSyncClientCapabilities Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/completion' request.
        /// </summary>
        [JsonPropertyName("completion")]
        public CompletionClientCapabilities Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/hover' request.
        /// </summary>
        [JsonPropertyName("hover")]
        public HoverClientCapabilities Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/signatureHelp' request.
        /// </summary>
        [JsonPropertyName("signatureHelp")]
        public SignatureHelpClientCapabilities SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/declaration' request.
        /// </summary>
        [JsonPropertyName("declaration")]
        public DeclarationClientCapabilities Declaration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/definition' request.
        /// </summary>
        [JsonPropertyName("definition")]
        public DefinitionClientCapabilities Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/typeDefinition' request.
        /// </summary>
        [JsonPropertyName("typeDefinition")]
        public TypeDefinitionClientCapabilities TypeDefinition { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/implementation' request.
        /// </summary>
        [JsonPropertyName("implementation")]
        public ImplementationClientCapabilities Implementation { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/references' request.
        /// </summary>
        [JsonPropertyName("references")]
        public ReferenceClientCapabilities References { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentHighlight' request.
        /// </summary>
        [JsonPropertyName("documentHighlight")]
        public DocumentHighlightClientCapabilities DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentSymbol' request.
        /// </summary>
        [JsonPropertyName("documentSymbol")]
        public DocumentSymbolClientCapabilities DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/codeAction' request.
        /// </summary>
        [JsonPropertyName("codeAction")]
        public CodeActionClientCapabilities CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/codeLens' request.
        /// </summary>
        [JsonPropertyName("codeLens")]
        public CodeLensClientCapabilities CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentLink' request.
        /// </summary>
        [JsonPropertyName("documentLink")]
        public DocumentLinkClientCapabilities DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/documentColor' and 'textDocument/colorPresentation' requests.
        /// </summary>
        [JsonPropertyName("colorProvider")]
        public DocumentColorClientCapabilities ColorProvider { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/formatting' request.
        /// </summary>
        [JsonPropertyName("formatting")]
        public DocumentFormattingClientCapabilities Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/rangeFormatting' request.
        /// </summary>
        [JsonPropertyName("rangeFormatting")]
        public DocumentRangeFormattingClientCapabilities RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/onTypeFormatting' request.
        /// </summary>
        [JsonPropertyName("onTypeFormatting")]
        public DocumentOnTypeFormattingClientCapabilities OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/rename' request.
        /// </summary>
        [JsonPropertyName("rename")]
        public RenameClientCapabilities Rename { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/publishDiagnostics' notification.
        /// </summary>
        [JsonPropertyName("publishDiagnostics")]
        public PublishDiagnosticsClientCapabilities PublishDiagnostics { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/foldingRange' request.
        /// </summary>
        [JsonPropertyName("foldingRange")]
        public FoldingRangeClientCapabilities FoldingRange { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/selectionRange' request.
        /// </summary>
        [JsonPropertyName("selectionRange")]
        public SelectionRangeClientCapabilities SelectionRange { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/linkedEditingRange' request.
        /// </summary>
        [JsonPropertyName("linkedEditingRange")]
        public LinkedEditingRangeClientCapabilities LinkedEditingRange { get; set; }

        /// <summary>
        /// Capabilities specific to the various call hierarchy requests.
        /// </summary>
        [JsonPropertyName("callHierarchy")]
        public CallHierarchyClientCapabilities CallHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the various semantic token requests.
        /// </summary>
        [JsonPropertyName("semanticToken")]
        public SemanticTokenClientCapabilities SemanticToken { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/moniker' request.
        /// </summary>
        [JsonPropertyName("moniker")]
        public MonikerClientCapabilities Moniker { get; set; }

        /// <summary>
        /// Capabilities specific to the various type hierarchy requests.
        /// </summary>
        [JsonPropertyName("typeHierarchy")]
        public TypeHierarchyClientCapabilities TypeHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/inlineValue' request.
        /// </summary>
        [JsonPropertyName("inlineValue")]
        public InlineValueClientCapabilities InlineValue { get; set; }

        /// <summary>
        /// Capabilities specific to the 'textDocument/inlayHint' request.
        /// </summary>
        [JsonPropertyName("inlayHint")]
        public InlayHintClientCapabilities InlayHint { get; set; }

        /// <summary>
        /// Capabilities specific to the diagnostic pull model.
        /// </summary>
        [JsonPropertyName("diagnostic")]
        public DiagnosticClientCapabilities Diagnostic { get; set; }
    }
}

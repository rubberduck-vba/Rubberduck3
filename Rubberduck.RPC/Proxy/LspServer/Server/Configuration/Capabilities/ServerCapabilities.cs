using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Configuration.Options;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Configuration;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Capabilities
{
    public class ServerCapabilities : SharedServerCapabilities
    {
        [JsonPropertyName("positionEncoding"), LspCompliant]
        public string PositionEncodingKind { get; set; } = Constants.PositionEncodingKind.UTF16;

        [JsonPropertyName("textDocumentSync"), LspCompliant]
        public TextDocumentSyncOptions TextDocumentSync { get; set; }

        //[JsonPropertyName("notebookDocumentSyncOptions")]
        //public object NotebookDocumentSyncOptions { get; set; } // LSP method not implemented

        [JsonPropertyName("completionProvider"), LspCompliant]
        public CompletionOptions CompletionProvider { get; set; }

        [JsonPropertyName("hoverProvider"), LspCompliant]
        public HoverOptions HoverProvider { get; set; }

        [JsonPropertyName("signatureHelpProvider"), LspCompliant]
        public SignatureHelpOptions SignatureHelpProvider { get; set; }

        [JsonPropertyName("declarationProvider"), LspCompliant]
        public DeclarationOptions DeclarationProvider { get; set; }

        [JsonPropertyName("definitionProvider"), LspCompliant]
        public DefinitionOptions DefinitionProvider { get; set; }

        [JsonPropertyName("typeDefinitionProvider"), LspCompliant]
        public TypeDefinitionOptions TypeDefinitionProvider { get; set; }

        [JsonPropertyName("implementationProvider"), LspCompliant]
        public ImplementationOptions ImplementationProvider { get; set; }

        [JsonPropertyName("referenceProvider"), LspCompliant]
        public ReferenceOptions ReferenceProvider { get; set; }

        [JsonPropertyName("documentHighlightProvider"), LspCompliant]
        public DocumentHighlightOptions DocumentHighlightProvider { get; set; }

        [JsonPropertyName("documentSymbolProvider"), LspCompliant]
        public DocumentSymbolOptions DocumentSymbolProvider { get; set; }

        [JsonPropertyName("codeActionProvider"), LspCompliant]
        public CodeActionOptions CodeActionProvider { get; set; }

        [JsonPropertyName("codeLensProvider"), LspCompliant]
        public CodeLensOptions CodeLensProvider { get; set; }

        [JsonPropertyName("documentLinkProvider"), LspCompliant]
        public DocumentLinkOptions DocumentLinkProvider { get; set; }

        [JsonPropertyName("colorProvider"), LspCompliant]
        public DocumentColorOptions ColorProvider { get; set; }

        [JsonPropertyName("documentFormattingProvider"), LspCompliant]
        public DocumentFormattingOptions DocumentFormattingProvider { get; set; }

        [JsonPropertyName("documentRangeFormattingProvider"), LspCompliant]
        public DocumentRangeFormattingOptions DocumentRangeFormattingProvider { get; set; }

        [JsonPropertyName("documentOnTypeFormattingProvider"), LspCompliant]
        public DocumentOnTypeFormattingOptions DocumentOnTypeFormattingProvider { get; set; }

        [JsonPropertyName("renameProvider"), LspCompliant]
        public RenameOptions RenameProvider { get; set; }

        [JsonPropertyName("foldingRangeProvider"), LspCompliant]
        public FoldingRangeOptions FoldingRangeProvider { get; set; }

        [JsonPropertyName("executeCommandProvider"), LspCompliant]
        public ExecuteCommandOptions ExecuteCommandProvider { get; set; }

        [JsonPropertyName("selectionRangeProvider"), LspCompliant]
        public SelectionRangeOptions SelectionRangeProvider { get; set; }

        [JsonPropertyName("linkedEditingRangeProvider"), LspCompliant]
        public LinkedEditingRangeOptions LinkedEditingRangeProvider { get; set; }

        [JsonPropertyName("callHierarchyProvider"), LspCompliant]
        public CallHierarchyOptions CallHierarchyProvider { get; set; }

        [JsonPropertyName("semanticTokensProvider"), LspCompliant]
        public SemanticTokensOptions SemanticTokensProvider { get; set; }

        [JsonPropertyName("monikerProvider"), LspCompliant]
        public MonikerOptions MonikerProvider { get; set; }

        [JsonPropertyName("typeHierarchyProvider"), LspCompliant]
        public TypeHierarchyOptions TypeHierarchyProvider { get; set; }

        [JsonPropertyName("inlineValueProvider"), LspCompliant]
        public InlineValueOptions InlineValueProvider { get; set; }

        [JsonPropertyName("inlayHintProvider"), LspCompliant]
        public InlayHintOptions InlayHintProvider { get; set; }

        [JsonPropertyName("diagnosticProvider"), LspCompliant]
        public DiagnosticOptions DiagnosticProvider { get; set; }

        [JsonPropertyName("workspaceSymbolProvider"), LspCompliant]
        public WorkspaceSymbolOptions WorkspaceSymbolProvider { get; set; }

        [JsonPropertyName("workspace"), LspCompliant]
        public WorkspaceServerCapabilities Workspace { get; set; }

        [JsonPropertyName("telemetry"), LspCompliant]
        public TelemetryOptions Telemetry { get; set; }

        [JsonPropertyName("experimental"), LspCompliant]
        public object Experimental { get; set; }
    }
}

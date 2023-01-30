using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration.Client
{
    public class DocumentSymbolClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'symbolKind' in the 'textDocument/documentSymbol' request.
        /// </summary>
        [JsonPropertyName("symbolKind"), LspCompliant]
        public SymbolKinds SymbolKind { get; set; }

        /// <summary>
        /// Whether the client supports hierarchical document symbols.
        /// </summary>
        [JsonPropertyName("hierarchicalDocumentSymbolSupport"), LspCompliant]
        public bool SupportsHierarchicalSymbols { get; set; }

        /// <summary>
        /// Tags supported on 'SymbolInformation'.
        /// </summary>
        [JsonPropertyName("tagSupport"), LspCompliant]
        public SupportedSymbolTags SupportedTags { get; set; }

        /// <summary>
        /// Whether the client supports an additional label presented in the UI when registering a document symbol provider.
        /// </summary>
        [JsonPropertyName("labelSupport"), LspCompliant]
        public bool SupportsLabel { get; set; }

        public class SymbolKinds
        {
            /// <summary>
            /// The <c>Constants.SymbolKind</c> values the client supports.
            /// </summary>
            [JsonPropertyName("valueSet"), LspCompliant]
            public Constants.SymbolKind.AsEnum[] ValueSet { get; set; }
        }

        public class SupportedSymbolTags
        {
            /// <summary>
            /// The <c>Constants.SymbolTag</c> values the client supports.
            /// </summary>
            [JsonPropertyName("valueSet"), LspCompliant]
            public Constants.SymbolTag.AsEnum[] valueSet { get; set; }
        }
    }
}

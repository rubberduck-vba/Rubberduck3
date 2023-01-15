using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DocumentSymbolClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'symbolKind' in the 'textDocument/documentSymbol' request.
        /// </summary>
        [JsonPropertyName("symbolKind")]
        public SymbolKinds SymbolKind { get; set; }

        /// <summary>
        /// Whether the client supports hierarchical document symbols.
        /// </summary>
        [JsonPropertyName("hierarchicalDocumentSymbolSupport")]
        public bool SupportsHierarchicalSymbols { get; set; }

        /// <summary>
        /// Tags supported on 'SymbolInformation'.
        /// </summary>
        [JsonPropertyName("tagSupport")]
        public SupportedSymbolTags SupportedTags { get; set; }

        /// <summary>
        /// Whether the client supports an additional label presented in the UI when registering a document symbol provider.
        /// </summary>
        [JsonPropertyName("labelSupport")]
        public bool SupportsLabel { get; set; }

        public class SymbolKinds
        {
            /// <summary>
            /// The <c>Constants.SymbolKind</c> values the client supports.
            /// </summary>
            [JsonPropertyName("valueSet")]
            public int[] ValueSet { get; set; }
        }

        public class SupportedSymbolTags
        {
            /// <summary>
            /// The <c>Constants.SymbolTag</c> values the client supports.
            /// </summary>
            [JsonPropertyName("valueSet")]
            public int[] valueSet { get; set; }
        }
    }
}

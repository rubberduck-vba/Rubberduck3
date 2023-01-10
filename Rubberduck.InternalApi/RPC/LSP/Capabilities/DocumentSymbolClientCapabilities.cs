using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentSymbolClientCapabilities")]
    public class DocumentSymbolClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'symbolKind' in the 'textDocument/documentSymbol' request.
        /// </summary>
        [ProtoMember(2, Name = "symbolKind")]
        public SymbolKinds SymbolKind { get; set; }

        /// <summary>
        /// Whether the client supports hierarchical document symbols.
        /// </summary>
        [ProtoMember(3, Name = "hierarchicalDocumentSymbolSupport")]
        public bool SupportsHierarchicalSymbols { get; set; }

        /// <summary>
        /// Tags supported on 'SymbolInformation'.
        /// </summary>
        [ProtoMember(4, Name = "tagSupport")]
        public SupportedSymbolTags SupportedTags { get; set; }

        /// <summary>
        /// Whether the client supports an additional label presented in the UI when registering a document symbol provider.
        /// </summary>
        [ProtoMember(5, Name = "labelSupport")]
        public bool SupportsLabel { get; set; }

        [ProtoContract(Name = "symbolKinds")]
        public class SymbolKinds
        {
            /// <summary>
            /// The <c>Constants.SymbolKind</c> values the client supports.
            /// </summary>
            [ProtoMember(1, Name = "valueSet")]
            public int[] ValueSet { get; set; }
        }

        [ProtoContract(Name = "supportedSymbolTags")]
        public class SupportedSymbolTags
        {
            /// <summary>
            /// The <c>Constants.SymbolTag</c> values the client supports.
            /// </summary>
            [ProtoMember(1, Name = "valueSet")]
            public int[] valueSet { get; set; }
        }
    }
}

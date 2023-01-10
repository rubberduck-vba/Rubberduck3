using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "workspaceSymbolClientCapabilities")]
    public class WorkspaceSymbolClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for symbol requests.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Specific capabilities for the <c>SymbolKind</c> in the 'workspace/symbol' request.
        /// </summary>
        [ProtoMember(2, Name = "symbolKind")]
        public SupportedSymbols SymbolKind { get; set; }

        /// <summary>
        /// The client supports tags on <c>SymbolInformation</c> and <c>WorkspaceSymbol</c>.
        /// </summary>
        [ProtoMember(3, Name = "tagSupport")]
        public SupportedTags TagSupport { get; set; } = SupportedTags.Default;

        /// <summary>
        /// Client supports partial workspace symbols and sends 'workspaceSymbol/resolve' requests to resolve additional properties.
        /// </summary>
        [ProtoMember(4, Name = "resolveSupport")]
        public PartialResolutionSupport ResolveSupport { get; set; }

        [ProtoContract(Name = "partialResolutionSupport")]
        public class PartialResolutionSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily. Usually 'location.range'.
            /// </summary>
            [ProtoMember(1, Name = "properties")]
            public string[] Properties { get; set; }
        }

        [ProtoContract(Name = "supportedSymbols")]
        public class SupportedSymbols
        {
            /// <summary>
            /// The <c>SymbolKind</c> values the client supports.
            /// </summary>
            /// <remarks>
            /// Client must gracefully handle values outside this set by falling back to a default when unknown.
            /// </remarks>
            [ProtoMember(1, Name = "valueSet")]
            public int[] ValueSet { get; set; }
        }

        [ProtoContract(Name = "supportedTags")]
        public class SupportedTags
        {
            public static SupportedTags Default { get; } = new SupportedTags { ValueSet = new[] { Constants.SymbolTag.Deprecated } };

            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [ProtoMember(1, Name = "valueSet")]
            public int[] ValueSet { get; set; }
        }
    }
}

using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Client
{
    public class WorkspaceSymbolClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for symbol requests.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Specific capabilities for the <c>SymbolKind</c> in the 'workspace/symbol' request.
        /// </summary>
        [JsonPropertyName("symbolKind"), LspCompliant]
        public SupportedSymbols SymbolKind { get; set; }

        /// <summary>
        /// The client supports tags on <c>SymbolInformation</c> and <c>WorkspaceSymbol</c>.
        /// </summary>
        [JsonPropertyName("tagSupport"), LspCompliant]
        public SupportedTags TagSupport { get; set; } = SupportedTags.Default;

        /// <summary>
        /// Client supports partial workspace symbols and sends 'workspaceSymbol/resolve' requests to resolve additional properties.
        /// </summary>
        [JsonPropertyName("resolveSupport"), LspCompliant]
        public PartialResolutionSupport ResolveSupport { get; set; }

        public class PartialResolutionSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily. Usually 'location.range'.
            /// </summary>
            [JsonPropertyName("properties"), LspCompliant]
            public string[] Properties { get; set; }
        }

        public class SupportedSymbols
        {
            /// <summary>
            /// The <c>SymbolKind</c> values the client supports.
            /// </summary>
            /// <remarks>
            /// Client must gracefully handle values outside this set by falling back to a default when unknown.
            /// </remarks>
            [JsonPropertyName("valueSet"), LspCompliant]
            public Constants.SymbolKind.AsEnum[] ValueSet { get; set; }
        }

        public class SupportedTags
        {
            public static SupportedTags Default { get; } 
                = new SupportedTags 
                { 
                    ValueSet = new[] { Constants.SymbolTag.AsEnum.Deprecated } 
                };

            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet"), LspCompliant]
            public Constants.SymbolTag.AsEnum[] ValueSet { get; set; }
        }
    }
}

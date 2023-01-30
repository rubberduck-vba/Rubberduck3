using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration.Client
{
    public class SemanticTokenClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Which requests the client supports and might send to the server depending on the server's capabilities.
        /// </summary>
        [JsonPropertyName("requests"), LspCompliant]
        public SupportedRequests Requests { get; set; }

        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        [JsonPropertyName("tokenTypes"), LspCompliant]
        public string[] TokenTypes { get; set; }
        
        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        [JsonPropertyName("tokenModifiers"), LspCompliant]
        public string[] TokenModifiers { get; set; }
        
        /// <summary>
        /// See <c>Constants.TokenFormat</c>
        /// </summary>
        [JsonPropertyName("tokenFormat"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.TokenFormat.AsStringEnum[] TokenFormat { get; set; } = new[] { Constants.TokenFormat.AsStringEnum.Relative };

        /// <summary>
        /// Whether the client supports tokens that can overlap each other.
        /// </summary>
        [JsonPropertyName("overlappingTokenSupport"), LspCompliant]
        public bool SupportsOverlappingTokens { get; set; }

        /// <summary>
        /// Whether the client supports tokens that can span multiple lines.
        /// </summary>
        [JsonPropertyName("multilineTokenSupport"), LspCompliant]
        public bool SupportsMultilineTokens { get; set; }

        /// <summary>
        /// Whether the client allows the server to actively cancel a semantic token request,
        /// </summary>
        [JsonPropertyName("serverCancellationSupport"), LspCompliant]
        public bool SupportsServerCancellation { get; set; }

        /// <summary>
        /// Whether the client uses semantic tokens to augment existing syntax tokens.
        /// If <c>true</c>, client side created syntax tokens and semantic tokens are both used for colorization.
        /// If <c>false</c>, client only uses returned semantic tokens for colorization.
        /// </summary>
        [JsonPropertyName("augmentsSyntaxTokens"), LspCompliant]
        public bool AugmentSyntaxTokens { get; set; }

        public class SupportedRequests
        {
            /// <summary>
            /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range' request if the server provides a corresponding handler.
            /// </summary>
            [JsonPropertyName("range"), LspCompliant]
            public bool Range { get; set; }

            /// <summary>
            /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range/full' request if the server provides a corresponding handler.
            /// </summary>
            [JsonPropertyName("full"), LspCompliant]
            public bool Full { get; set; }
        }

        public class SupportsFullRequestsWithDelta : SupportedRequests
        {
            [JsonPropertyName("full"), LspCompliant]
            public new SupportsDelta Full { get; set; }

            public class SupportsDelta
            {
                /// <summary>
                /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range/full/delta' request if the server provides a corresponding handler.
                /// </summary>
                [JsonPropertyName("delta")]
                public bool Delta { get; set; }
            }
        }
    }
}

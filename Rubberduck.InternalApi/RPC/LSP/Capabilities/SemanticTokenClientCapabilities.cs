using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class SemanticTokenClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Which requests the client supports and might send to the server depending on the server's capabilities.
        /// </summary>
        [JsonPropertyName("requests")]
        public SupportedRequests Requests { get; set; }

        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        [JsonPropertyName("tokenTypes")]
        public string[] TokenTypes { get; set; }

        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        [JsonPropertyName("tokenModifiers")]
        public string[] TokenModifiers { get; set; }

        /// <summary>
        /// See <c>Constants.TokenFormat</c>
        /// </summary>
        [JsonPropertyName("tokenFormat")]
        public string[] TokenFormat { get; set; } = new[] { Constants.TokenFormat.Relative };

        /// <summary>
        /// Whether the client supports tokens that can overlap each other.
        /// </summary>
        [JsonPropertyName("overlappingTokenSupport")]
        public bool SupportsOverlappingTokens { get; set; }

        /// <summary>
        /// Whether the client supports tokens that can span multiple lines.
        /// </summary>
        [JsonPropertyName("multilineTokenSupport")]
        public bool SupportsMultilineTokens { get; set; }

        /// <summary>
        /// Whether the client allows the server to actively cancel a semantic token request,
        /// </summary>
        [JsonPropertyName("serverCancellationSupport")]
        public bool SupportsServerCancellation { get; set; }

        /// <summary>
        /// Whether the client uses semantic tokens to augment existing syntax tokens.
        /// If <c>true</c>, client side created syntax tokens and semantic tokens are both used for colorization.
        /// If <c>false</c>, client only uses returned semantic tokens for colorization.
        /// </summary>
        [JsonPropertyName("augmentsSyntaxTokens")]
        public bool AugmentSyntaxTokens { get; set; }

        public class SupportedRequests
        {
            /// <summary>
            /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range' request if the server provides a corresponding handler.
            /// </summary>
            [JsonPropertyName("range")]
            public bool Range { get; set; }

            /// <summary>
            /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range/full' request if the server provides a corresponding handler.
            /// </summary>
            [JsonPropertyName("full")]
            public bool Full { get; set; }
        }

        public class SupportsFullRequestsWithDelta : SupportedRequests
        {
            [JsonPropertyName("full")]
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

using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "semanticTokenClientCapabilities")]
    public class SemanticTokenClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Which requests the client supports and might send to the server depending on the server's capabilities.
        /// </summary>
        [ProtoMember(2, Name = "requests")]
        public SupportedRequests Requests { get; set; }

        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        [ProtoMember(3, Name = "tokenTypes")]
        public string[] TokenTypes { get; set; }

        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        [ProtoMember(4, Name = "tokenModifiers")]
        public string[] TokenModifiers { get; set; }

        /// <summary>
        /// See <c>Constants.TokenFormat</c>
        /// </summary>
        [ProtoMember(5, Name = "tokenFormat")]
        public string[] TokenFormat { get; set; } = new[] { Constants.TokenFormat.Relative };

        /// <summary>
        /// Whether the client supports tokens that can overlap each other.
        /// </summary>
        [ProtoMember(6, Name = "overlappingTokenSupport")]
        public bool SupportsOverlappingTokens { get; set; }

        /// <summary>
        /// Whether the client supports tokens that can span multiple lines.
        /// </summary>
        [ProtoMember(7, Name = "multilineTokenSupport")]
        public bool SupportsMultilineTokens { get; set; }

        /// <summary>
        /// Whether the client allows the server to actively cancel a semantic token request,
        /// </summary>
        [ProtoMember(8, Name = "serverCancellationSupport")]
        public bool SupportsServerCancellation { get; set; }

        /// <summary>
        /// Whether the client uses semantic tokens to augment existing syntax tokens.
        /// If <c>true</c>, client side created syntax tokens and semantic tokens are both used for colorization.
        /// If <c>false</c>, client only uses returned semantic tokens for colorization.
        /// </summary>
        [ProtoMember(9, Name = "augmentsSyntaxTokens")]
        public bool AugmentSyntaxTokens { get; set; }

        [ProtoContract(Name = "supportedRequests")]
        public class SupportedRequests
        {
            /// <summary>
            /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range' request if the server provides a corresponding handler.
            /// </summary>
            [ProtoMember(1, Name = "range")]
            public bool Range { get; set; }

            /// <summary>
            /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range/full' request if the server provides a corresponding handler.
            /// </summary>
            [ProtoMember(2, Name = "full")]
            public bool Full { get; set; }
        }

        [ProtoContract(Name = "supportsFullRequestsWithDelta")]
        public class SupportsFullRequestsWithDelta : SupportedRequests
        {
            [ProtoMember(1, Name = "full")]
            public new SupportsDelta Full { get; set; }

            [ProtoContract(Name = "supportsDelta")]
            public class SupportsDelta
            {
                /// <summary>
                /// If <c>true</c>, client will send a 'textDocument/semanticTokens/range/full/delta' request if the server provides a corresponding handler.
                /// </summary>
                [ProtoMember(1, Name = "delta")]
                public bool Delta { get; set; }
            }
        }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class CodeActionClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// If supplied, the client supports code action literals as a valid response of the 'textDocument/codeAction' request.
        /// </summary>
        [JsonPropertyName("codeActionLiteralSupport")]
        public CodeActionLiteralSupport SupportsCodeActionLiteral { get; set; }

        /// <summary>
        /// Whether the client supports the 'isPreferred' code action property.
        /// </summary>
        [JsonPropertyName("isPreferredSupport")]
        public bool SupportsIsPreferred { get; set; }

        /// <summary>
        /// Whether the client supports the 'isDisabled' code action property.
        /// </summary>
        [JsonPropertyName("isDisabledSupport")]
        public bool SupportsIsDisabled { get; set; }

        /// <summary>
        /// Whether the client supports the 'data' code action property.
        /// </summary>
        /// <remarks>
        /// If supported, the data is preserved between a 'textDocument/codeAction' and a 'codeAction/resolve' request.
        /// </remarks>
        [JsonPropertyName("dataSupport")]
        public bool SupportsData { get; set; }

        /// <summary>
        /// Whether the client supports resolving additional code action properties via a separate 'codeAction/resolve' request.
        /// </summary>
        [JsonPropertyName("resolveSupport")]
        public PropertyResolutionSupport SupportsPropertyResolution { get; set; }

        /// <summary>
        /// Whether the client honors the change annotations in text edits and resource operations returned via the 'CodeAction#edit' property.
        /// </summary>
        [JsonPropertyName("honorsChangeAnnotations")]
        public bool HonorsChangeAnnotations { get; set; }

        public class PropertyResolutionSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            [JsonPropertyName("properties")]
            public string[] Properties { get; set; }
        }

        public class CodeActionLiteralSupport
        {
            [JsonPropertyName("codeActionKind")]
            public CodeActionKindSupport CodeActionKind { get; set; }

            public class CodeActionKindSupport
            {
                /// <summary>
                /// The <c>Constants.CodeActionKind</c> values supported by the client.
                /// </summary>
                [JsonPropertyName("valueSet")]
                public string[] ValueSet { get; set; }
            }
        }
    }
}

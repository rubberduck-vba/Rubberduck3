using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class PublishDiagnosticsClientCapabilities
    {
        /// <summary>
        /// Whether the client accepts diagnostics with related information.
        /// </summary>
        [JsonPropertyName("relatedInformation")]
        public bool RelatedInformation { get; set; }

        /// <summary>
        /// Whether client supports the 'tag' property to provide metadata about a diagnostic.
        /// </summary>
        [JsonPropertyName("supportedTags")]
        public TagSupport SupportedTags { get; set; }

        public class TagSupport
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet")]
            public int[] ValueSet { get; set; }
        }

        /// <summary>
        /// Whether the client interprets the version property of the 'textDocument/publishDiagnostics' notification parameter.
        /// </summary>
        [JsonPropertyName("versionSupport")]
        public bool SupportsVersion { get; set; }

        /// <summary>
        /// Whether the client supports a 'codeDescription' property.
        /// </summary>
        [JsonPropertyName("codeDescriptionSupport")]
        public bool SupportsCodeDescription { get; set; }

        /// <summary>
        /// Whether code actions support the 'data' property (preserved between 'textDocument/publishDiagnostics' and 'textDocument/codeAction' requests).
        /// </summary>
        [JsonPropertyName("dataSupport")]
        public bool SupportsData { get; set; }
    }
}

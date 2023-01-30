using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration.Client
{
    public class PublishDiagnosticsClientCapabilities
    {
        /// <summary>
        /// Whether the client accepts diagnostics with related information.
        /// </summary>
        [JsonPropertyName("relatedInformation"), LspCompliant]
        public bool RelatedInformation { get; set; }

        /// <summary>
        /// Whether client supports the 'tag' property to provide metadata about a diagnostic.
        /// </summary>
        [JsonPropertyName("supportedTags"), LspCompliant]
        public TagSupport SupportedTags { get; set; }

        public class TagSupport
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet"), LspCompliant]
            public Constants.DiagnosticTags.AsEnum[] ValueSet { get; set; }
        }

        /// <summary>
        /// Whether the client interprets the version property of the 'textDocument/publishDiagnostics' notification parameter.
        /// </summary>
        [JsonPropertyName("versionSupport"), LspCompliant]
        public bool SupportsVersion { get; set; }

        /// <summary>
        /// Whether the client supports a 'codeDescription' property.
        /// </summary>
        [JsonPropertyName("codeDescriptionSupport"), LspCompliant]
        public bool SupportsCodeDescription { get; set; }

        /// <summary>
        /// Whether code actions support the 'data' property (preserved between 'textDocument/publishDiagnostics' and 'textDocument/codeAction' requests).
        /// </summary>
        [JsonPropertyName("dataSupport"), LspCompliant]
        public bool SupportsData { get; set; }
    }
}

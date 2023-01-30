using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration.Client
{
    public class SignatureHelpClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Describes client support for signature info.
        /// </summary>
        [JsonPropertyName("signatureInformation")]
        public SignatureInformationCapabilities SignatureInformation { get; set; }

        [JsonPropertyName("contextSupport")]
        public bool SupportsContext { get; set; }

        public class SignatureInformationCapabilities
        {
            /// <summary>
            /// Supported content formats for the 'documentation' property.
            /// </summary>
            /// <remarks>
            /// See <c>Constants.MarkupKind</c> for values.
            /// </remarks>
            [JsonPropertyName("documentationFormat")]
            public int[] DocumentationFormat { get; set; }

            /// <summary>
            /// Client capabilities specific to parameter information.
            /// </summary>
            [JsonPropertyName("parameterInformation")]
            public ParameterInformationCapabilities ParameterInformation { get; set; }

            /// <summary>
            /// <c>true</c> if the client supports the 'activeParameter' property on 'SignatureInformation' literals.
            /// </summary>
            [JsonPropertyName("activeParameterSupport")]
            public bool SupportsActiveParameter { get; set; }

            /// <summary>
            /// <c>true</c> if client supports additional context information for a 'textDocument/signatureHelp' request.
            /// </summary>
            [JsonPropertyName("contextSupport")]
            public bool SupportsContext { get; set; }

            public class ParameterInformationCapabilities
            {
                /// <summary>
                /// <c>true</c> if the client supports processing label offsets instead of a simple label string.
                /// </summary>
                [JsonPropertyName("supportsLabelOffset")]
                public bool SupportsLabelOffset { get; set; }
            }
        }
    }
}

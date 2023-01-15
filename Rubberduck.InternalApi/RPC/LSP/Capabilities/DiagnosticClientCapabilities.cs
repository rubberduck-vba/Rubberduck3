using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DiagnosticClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports related documents for document diagnostic pulls.
        /// </summary>
        [JsonPropertyName("relatedDocumentSupport")]
        public bool SupportsRelatedDocument { get; set; }
    }
}

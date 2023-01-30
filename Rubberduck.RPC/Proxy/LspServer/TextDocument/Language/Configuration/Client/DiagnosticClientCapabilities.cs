using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration.Client
{
    public class DiagnosticClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports related documents for document diagnostic pulls.
        /// </summary>
        [JsonPropertyName("relatedDocumentSupport")]
        public bool SupportsRelatedDocument { get; set; }
    }
}

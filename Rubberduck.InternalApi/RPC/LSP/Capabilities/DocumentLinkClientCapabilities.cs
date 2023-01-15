using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DocumentLinkClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports the 'tooltip' property on 'DocumentLink'.
        /// </summary>
        [JsonPropertyName("tooltipSupport")]
        public bool SupportsToolTip { get; set; }
    }
}

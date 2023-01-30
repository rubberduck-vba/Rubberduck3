using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Configuration.Client
{
    public class DocumentLinkClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports the 'tooltip' property on 'DocumentLink'.
        /// </summary>
        [JsonPropertyName("tooltipSupport"), LspCompliant]
        public bool SupportsToolTip { get; set; }
    }
}

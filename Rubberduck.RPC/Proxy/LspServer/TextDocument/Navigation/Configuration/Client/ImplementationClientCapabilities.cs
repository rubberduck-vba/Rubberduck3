using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Configuration.Client
{
    public class ImplementationClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports additional metadata in the form of definition links.
        /// </summary>
        [JsonPropertyName("linkSupport"), LspCompliant]
        public bool SupportsLinks { get; set; }
    }
}

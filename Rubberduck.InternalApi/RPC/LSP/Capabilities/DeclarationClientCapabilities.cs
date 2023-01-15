using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DeclarationClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports additional metadata in the form of declaration links.
        /// </summary>
        [JsonPropertyName("linkSupport")]
        public bool SupportsLinks { get; set; }
    }
}

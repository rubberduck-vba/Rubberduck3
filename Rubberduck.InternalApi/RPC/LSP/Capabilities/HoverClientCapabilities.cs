using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class HoverClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// See <c>MarkupKind</c> constants.
        /// </summary>
        [JsonPropertyName("contentFormat")]
        public int[] ContentFormat { get; set; }
    }
}

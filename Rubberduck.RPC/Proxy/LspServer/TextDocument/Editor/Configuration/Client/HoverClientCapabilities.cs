using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration.Client
{
    public class HoverClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// The content format kind.
        /// </summary>
        [JsonPropertyName("contentFormat"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.MarkupKind.AsStringEnum[] ContentFormat { get; set; }
    }
}

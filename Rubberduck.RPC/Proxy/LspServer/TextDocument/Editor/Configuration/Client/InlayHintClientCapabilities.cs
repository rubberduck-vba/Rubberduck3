using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration.Client
{
    public class InlayHintClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Indicates which properties a client can resolve lazily on a inlay hint.
        /// </summary>
        [JsonPropertyName("resolveSupport")]
        public LazyResolutionSupport SupportsLazyResolution { get; set; }

        public class LazyResolutionSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            [JsonPropertyName("properties")]
            public string[] Properties { get; set; }
        }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DidChangeWatchedFilesClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for DidChangeWatchedFiles notifications.
        /// </summary>
        /// <remarks>
        /// Protocol does not support static configuration for server-side file changes.
        /// </remarks>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// <c>true</c> if the client supports relative patterns.
        /// </summary>
        [JsonPropertyName("relativePatternSupport")]
        public bool SupportsRelativePattern { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class TextDocumentSyncClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for text document synchronization.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether client supports sending willSave notifications.
        /// </summary>
        [JsonPropertyName("willSave")]
        public bool SupportsWillSave { get; set; }

        /// <summary>
        /// Whether client sending willSave request waits for a response providing textEdits to apply before saving the document.
        /// </summary>
        [JsonPropertyName("willSaveWaitUntil")]
        public bool SupportsWillSaveWaitUntil { get; set; }

        /// <summary>
        /// Whether the client supports sending didSave notifications.
        /// </summary>
        [JsonPropertyName("didSave")]
        public bool SupportsDidSave { get; set; }
    }
}

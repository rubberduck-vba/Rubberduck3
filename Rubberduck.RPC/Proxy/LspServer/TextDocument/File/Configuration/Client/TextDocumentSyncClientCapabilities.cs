using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Configuration.Client
{
    public class TextDocumentSyncClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for text document synchronization.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether client supports sending willSave notifications.
        /// </summary>
        [JsonPropertyName("willSave"), LspCompliant]
        public bool SupportsWillSave { get; set; }

        /// <summary>
        /// Whether client sending willSave request waits for a response providing textEdits to apply before saving the document.
        /// </summary>
        [JsonPropertyName("willSaveWaitUntil"), LspCompliant]
        public bool SupportsWillSaveWaitUntil { get; set; }

        /// <summary>
        /// Whether the client supports sending didSave notifications.
        /// </summary>
        [JsonPropertyName("didSave"), LspCompliant]
        public bool SupportsDidSave { get; set; }
    }
}

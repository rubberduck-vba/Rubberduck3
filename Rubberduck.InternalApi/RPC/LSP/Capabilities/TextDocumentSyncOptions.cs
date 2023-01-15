using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class TextDocumentSyncOptions
    {
        /// <summary>
        /// Whether open and close notifications are sent to the server.
        /// </summary>
        [JsonPropertyName("openClose")]
        public bool NotifyOpenClose { get; set; } = true;

        /// <summary>
        /// How change notifications are sent to the server.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.TextDocumentSyncKind</c>.
        /// </remarks>
        [JsonPropertyName("change")]
        public int ChangeNotificationSyncKind { get; set; }
    }
}

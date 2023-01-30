using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Options
{
    public class TextDocumentSyncOptions
    {
        /// <summary>
        /// Whether open and close notifications are sent to the server.
        /// </summary>
        [JsonPropertyName("openClose"), LspCompliant]
        public bool NotifyOpenClose { get; set; } = true;

        /// <summary>
        /// How change notifications are sent to the server.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.TextDocumentSyncKind</c>.
        /// </remarks>
        [JsonPropertyName("change"), LspCompliant]
        public Constants.TextDocumentSyncKind.AsEnum ChangeNotificationSyncKind { get; set; } = Constants.TextDocumentSyncKind.AsEnum.Full;
    }
}

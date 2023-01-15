using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class WorkspaceFoldersServerCapabilities
    {
        /// <summary>
        /// <c>true</c> if the server supports workspace folders.
        /// </summary>
        [JsonPropertyName("supported")]
        public bool IsSupported { get; set; }

        /// <summary>
        /// Whether the server wants to receive workspace folder change notifications.
        /// If a string other than 'true' or 'false' is provided, the string is treated as a client registration ID.
        /// </summary>
        [JsonPropertyName("changeNotifications")]
        public string ChangeNotifications { get; set; }
    }
}

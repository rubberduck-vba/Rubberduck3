using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class WindowClientCapabilities
    {
        /// <summary>
        /// Whether the client supports server-initiated progress using the 'window/workDoneProgress/create' request.
        /// </summary>
        /// <remarks>
        /// Also controls whether client supports handling progress notifications.
        /// </remarks>
        [JsonPropertyName("workDoneProgress")]
        public bool WorkDoneProgress { get; set; }

        /// <summary>
        /// Capabilities specific to the showMessage request.
        /// </summary>
        [JsonPropertyName("showMessage")]
        public ShowMessageRequestClientCapabilities ShowMessage { get; set; }

        /// <summary>
        /// Capabilities specific to the showDocument request.
        /// </summary>
        [JsonPropertyName("showDocument")]
        public ShowDocumentClientCapabilities ShowDocument { get; set; }
    }
}

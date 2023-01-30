using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Window.Configuration.Client
{
    public class WindowClientCapabilities
    {
        /// <summary>
        /// Whether the client supports server-initiated progress using the 'window/workDoneProgress/create' request.
        /// </summary>
        /// <remarks>
        /// Also controls whether client supports handling progress notifications.
        /// </remarks>
        [JsonPropertyName("workDoneProgress"), LspCompliant]
        public bool WorkDoneProgress { get; set; }

        /// <summary>
        /// Capabilities specific to the showMessage request.
        /// </summary>
        [JsonPropertyName("showMessage"), LspCompliant]
        public ShowMessageRequestClientCapabilities ShowMessage { get; set; }

        /// <summary>
        /// Capabilities specific to the showDocument request.
        /// </summary>
        [JsonPropertyName("showDocument"), LspCompliant]
        public ShowDocumentClientCapabilities ShowDocument { get; set; }
    }
}

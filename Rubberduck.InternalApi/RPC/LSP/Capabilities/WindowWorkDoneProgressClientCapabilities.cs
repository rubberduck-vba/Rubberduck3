using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class WindowWorkDoneProgressClientCapabilities
    {
        /// <summary>
        /// Whether client supports server-initiated progress via a 'window/workDoneProgress/create' request.
        /// </summary>
        [JsonPropertyName("workDoneProgress")]
        public bool WorkDoneProgress { get; set; }
    }
}

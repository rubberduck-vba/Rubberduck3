using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Window.Configuration.Client
{
    public class ShowDocumentClientCapabilities
    {
        /// <summary>
        /// <c>true</c> if the client supports showDocument requests.
        /// </summary>
        [JsonPropertyName("support")]
        public bool IsSupported { get; set; }
    }
}

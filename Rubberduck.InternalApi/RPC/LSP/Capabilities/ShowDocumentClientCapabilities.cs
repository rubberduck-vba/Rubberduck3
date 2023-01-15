using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
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

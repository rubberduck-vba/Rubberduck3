
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DidChangeConfigurationClientCapabilities
    {
        /// <summary>
        /// The modified configuration settings.
        /// </summary>
        [JsonPropertyName("settings")]
        public LSPAny Settings { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class RegularExpressionClientCapabilies
    {
        [JsonPropertyName("engine")]
        public string Engine { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}

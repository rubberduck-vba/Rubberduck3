using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ConfigurationParams
    {
        [JsonPropertyName("items")]
        public ConfigurationItem[] Items { get; set; }
    }
}

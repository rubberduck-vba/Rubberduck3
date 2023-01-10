using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "configurationParams")]
    public class ConfigurationParams
    {
        [JsonPropertyName("items")]
        [ProtoMember(1)]
        public ConfigurationItem[] Items { get; set; }
    }
}

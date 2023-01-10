using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "configurationItem")]
    public class ConfigurationItem
    {
        /// <summary>
        /// The scope to get the configuration section for.
        /// </summary>
        [JsonPropertyName("scopeUri")]
        [ProtoMember(1)]
        public string ScopeUri { get; set; }

        /// <summary>
        /// The requested configuration section.
        /// </summary>
        [JsonPropertyName("section")]
        [ProtoMember(2)]
        public string Section { get; set; }
    }
}

using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didChangeConfigurationParams")]
    public class DidChangeConfigurationParams
    {
        /// <summary>
        /// The changed settings.
        /// </summary>
        [JsonPropertyName("settings")]
        [ProtoMember(1)]
        public LSPAny Settings { get; set; }
    }
}

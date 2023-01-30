using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class ClientInitializedParams : InitializedParams
    {
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.DataServer.Parameters
{
    public class DisconnectParams
    {
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }
    }
}

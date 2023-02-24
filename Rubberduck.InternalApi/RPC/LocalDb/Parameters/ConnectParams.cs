using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.DataServer.Parameters
{
    public class ConnectParams
    {
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}

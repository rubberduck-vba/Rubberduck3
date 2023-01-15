using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.DataServer.Response
{
    public class ConnectResult
    {
        [JsonPropertyName("connected")]
        public bool Connected { get; set; }
    }
}

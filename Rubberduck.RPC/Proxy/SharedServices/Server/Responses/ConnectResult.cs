using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Responses
{
    public class ConnectResult
    {
        [JsonPropertyName("connected")]
        public bool Connected { get; set; }
    }
}

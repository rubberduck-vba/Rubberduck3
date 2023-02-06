using Rubberduck.RPC.Platform;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters
{
    public class LogMessageParams
    {
        [JsonPropertyName("messageType")]
        public Constants.MessageType.AsEnum MessageType { get; set; } = Constants.MessageType.AsEnum.Log;

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}

using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Window.Commands.Parameters
{
    public class ShowMessageParams
    {
        [JsonPropertyName("type"), LspCompliant]
        public Constants.MessageType.AsEnum MessageType { get; set; } = Constants.MessageType.AsEnum.Info;

        [JsonPropertyName("message"), LspCompliant]
        public string Message { get; set; }
    }
}

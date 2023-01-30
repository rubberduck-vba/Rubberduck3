using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Window.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Window.Commands.Parameters
{
    public class ShowMessageRequestParams : ShowMessageParams
    {
        [JsonPropertyName("actions"), LspCompliant]
        public MessageActionItem[] Actions { get; set; }
    }
}

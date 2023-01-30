using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Window.Model
{
    public class MessageActionItem
    {
        /// <summary>
        /// A short title like 'Retry', 'Open Log', etc.
        /// </summary>
        [JsonPropertyName("title"), LspCompliant]
        public string Title { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Client
{
    public class DidChangeConfigurationClientCapabilities
    {
        /// <summary>
        /// The modified configuration settings.
        /// </summary>
        [JsonPropertyName("settings")]
        public object Settings { get; set; }
    }
}

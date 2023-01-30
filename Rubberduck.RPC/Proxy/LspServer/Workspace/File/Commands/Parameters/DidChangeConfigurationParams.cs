using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters
{
    public class DidChangeConfigurationParams
    {
        /// <summary>
        /// The changed settings.
        /// </summary>
        [JsonPropertyName("settings")]
        public object Settings { get; set; }
    }
}

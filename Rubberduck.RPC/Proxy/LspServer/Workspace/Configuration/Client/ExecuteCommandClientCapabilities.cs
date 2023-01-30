using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Client
{
    public class ExecuteCommandClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for commands.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }
    }
}

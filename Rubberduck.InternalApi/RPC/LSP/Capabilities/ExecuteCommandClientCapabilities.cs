using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
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

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ConfigurationItem
    {
        /// <summary>
        /// The scope to get the configuration section for.
        /// </summary>
        [JsonPropertyName("scopeUri")]
        public string ScopeUri { get; set; }

        /// <summary>
        /// The requested configuration section.
        /// </summary>
        [JsonPropertyName("section")]
        public string Section { get; set; }
    }
}

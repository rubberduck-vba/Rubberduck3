using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters
{
    public class ConfigurationParams
    {
        [JsonPropertyName("items")]
        public ConfigurationItem[] Items { get; set; }

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
}

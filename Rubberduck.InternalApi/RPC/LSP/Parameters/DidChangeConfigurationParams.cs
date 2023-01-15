using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DidChangeConfigurationParams
    {
        /// <summary>
        /// The changed settings.
        /// </summary>
        [JsonPropertyName("settings")]
        public LSPAny Settings { get; set; }
    }
}

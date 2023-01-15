using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class PublishDiagnosticsParams
    {
        [JsonPropertyName("uri")]
        public string DocumentUri { get; set; }

        [JsonPropertyName("version")]
        public int? Version { get; set; }

        [JsonPropertyName("diagnostics")]
        public Diagnostic[] Diagnostics { get; set; }
    }
}

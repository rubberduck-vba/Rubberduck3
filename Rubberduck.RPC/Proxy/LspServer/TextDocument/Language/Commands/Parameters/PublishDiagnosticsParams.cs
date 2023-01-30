using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters
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

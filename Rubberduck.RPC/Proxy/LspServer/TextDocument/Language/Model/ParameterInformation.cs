using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class ParameterInformation
    {
        [JsonPropertyName("label"), LspCompliant]
        public string Label { get; set; }

        [JsonPropertyName("documentation"), LspCompliant]
        public string Documentation { get; set; }
    }
}

using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class ColorInformation
    {
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        [JsonPropertyName("color"), LspCompliant]
        public Color Color { get; set; }
    }
}

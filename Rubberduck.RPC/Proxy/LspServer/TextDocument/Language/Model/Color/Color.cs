using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class Color
    {
        [JsonPropertyName("red"), LspCompliant]
        public decimal Red { get; set; }

        [JsonPropertyName("green"), LspCompliant]
        public decimal Green { get; set; }

        [JsonPropertyName("blue"), LspCompliant]
        public decimal Blue { get; set; }

        [JsonPropertyName("alpha"), LspCompliant]
        public decimal Alpha { get; set; }
    }
}

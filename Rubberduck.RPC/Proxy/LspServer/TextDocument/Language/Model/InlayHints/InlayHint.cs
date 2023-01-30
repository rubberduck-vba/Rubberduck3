using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class InlayHint
    {
        [JsonPropertyName("position"), LspCompliant]
        public Position Position { get; set; }

        [JsonPropertyName("label"), LspCompliant]
        public InlayHintLabelPart Label { get; set; }

        [JsonPropertyName("kind"), LspCompliant]
        public Constants.InlayHintKind.AsEnum? Kind { get; set; }

        [JsonPropertyName("textEdits"), LspCompliant]
        public TextEdit[] TextEdits { get; set; }

        [JsonPropertyName("tooltip"), LspCompliant]
        public MarkupContent ToolTip { get; set; }

        [JsonPropertyName("paddingLeft"), LspCompliant]
        public bool PadLeft { get; set; }

        [JsonPropertyName("paddingRight"), LspCompliant]
        public bool PadRight { get; set; }

        [JsonPropertyName("data"), LspCompliant]
        public object Data { get; set; }
    }
}

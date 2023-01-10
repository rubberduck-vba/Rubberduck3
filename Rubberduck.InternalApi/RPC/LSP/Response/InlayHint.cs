using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class InlayHint
    {
        [JsonPropertyName("position")]
        public Position Position { get; set; }

        [JsonPropertyName("label")]
        public InlayHintLabelPart Label { get; set; }

        [JsonPropertyName("kind")]
        public int? Kind { get; set; }

        [JsonPropertyName("textEdits")]
        public TextEdit[] TextEdits { get; set; }

        [JsonPropertyName("tooltip")]
        public MarkupContent ToolTip { get; set; }

        [JsonPropertyName("paddingLeft")]
        public bool PadLeft { get; set; }

        [JsonPropertyName("paddingRight")]
        public bool PadRight { get; set; }

        [JsonPropertyName("data")]
        public LSPAny Data { get; set; }
    }
}

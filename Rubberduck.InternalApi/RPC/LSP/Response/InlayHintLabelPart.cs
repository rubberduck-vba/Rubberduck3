using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    /// <summary>
    /// Inlay hint label part allows for interactive and composite labels for inlay hints.
    /// </summary>
    public class InlayHintLabelPart
    {
        /// <summary>
        /// The value of this label part.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// The tooltip text when you hover this label part.
        /// </summary>
        /// <remarks>
        /// Client may resolve this property late with a separate resolve request.
        /// </remarks>
        [JsonPropertyName("tooltip")]
        public MarkupContent ToolTip { get; set; }

        /// <summary>
        /// An optional source code location representing this label part.
        /// </summary>
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        /// <summary>
        /// An optional command.
        /// </summary>
        [JsonPropertyName("command")]
        public Command Command { get; set; }
    }
}

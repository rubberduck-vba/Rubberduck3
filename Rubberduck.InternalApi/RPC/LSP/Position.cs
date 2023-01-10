using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Represents a zero-based caret position between two characters.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Zero-based line position in a document.
        /// </summary>
        [JsonPropertyName("line")]
        public uint Line { get; set; }
        /// <summary>
        /// Zero-based column position in a document. The meaning of this offset is determined by the negotiated 'PositionEncodingKind'.
        /// </summary>
        /// <remarks>If the character value is greater than the line length, line length is used.</remarks>
        [JsonPropertyName("character")]
        public uint Character { get; set; }
    }
}

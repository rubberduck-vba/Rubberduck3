using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// A range in a text document expressed as (zero-based) start and end positions.
    /// </summary>
    /// <remarks>
    /// <c>End</c> position is exclusive; use an <c>End</c> position denoting the start of the next line to specify a range that contains <em>and includes</em> a line ending.
    /// </remarks>
    public class Range
    {
        /// <summary>
        /// The range's start position.
        /// </summary>
        [JsonPropertyName("start")]
        public Position Start { get; set; }

        /// <summary>
        /// The range's end position.
        /// </summary>
        [JsonPropertyName("end")]
        public Position End { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class FoldingRange
    {
        /// <summary>
        /// The zero-based start line of the range to fold.
        /// </summary>
        [JsonPropertyName("startLine")]
        public uint StartLine { get; set; }

        /// <summary>
        /// The zero-based character offset from where the folded range starts.
        /// </summary>
        /// <remarks>
        /// <c>null</c> defaults to the length of the start line.
        /// </remarks>
        [JsonPropertyName("startCharacter")]
        public uint? StartCharacter { get; set; }

        /// <summary>
        /// The zero-based end line of the range to fold.
        /// </summary>
        [JsonPropertyName("endLine")]
        public uint EndLine { get; set; }

        /// <summary>
        /// The zero-based character offset before the folded range ends.
        /// </summary>
        /// <remarks>
        /// <c>null</c> defaults to the length of the end line.
        /// </remarks>
        [JsonPropertyName("endCharacter")]
        public uint? EndCharacter { get; set; }

        /// <summary>
        /// Describes the kind of folding range such as 'comment' or 'region'.
        /// </summary>
        /// <remarks>
        /// Used by commands such as 'fold all comments'.
        /// </remarks>
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// The text that the client should show when the specified range is collapsed.
        /// </summary>
        [JsonPropertyName("collapsedText")]
        public string CollapsedText { get; set; }
    }
}

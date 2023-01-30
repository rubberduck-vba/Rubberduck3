using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Model
{
    public class FoldingRange
    {
        /// <summary>
        /// The zero-based start line of the range to fold.
        /// </summary>
        [JsonPropertyName("startLine"), LspCompliant]
        public uint StartLine { get; set; }

        /// <summary>
        /// The zero-based character offset from where the folded range starts.
        /// </summary>
        /// <remarks>
        /// <c>null</c> defaults to the length of the start line.
        /// </remarks>
        [JsonPropertyName("startCharacter"), LspCompliant]
        public uint? StartCharacter { get; set; }

        /// <summary>
        /// The zero-based end line of the range to fold.
        /// </summary>
        [JsonPropertyName("endLine"), LspCompliant]
        public uint EndLine { get; set; }

        /// <summary>
        /// The zero-based character offset before the folded range ends.
        /// </summary>
        /// <remarks>
        /// <c>null</c> defaults to the length of the end line.
        /// </remarks>
        [JsonPropertyName("endCharacter"), LspCompliant]
        public uint? EndCharacter { get; set; }

        /// <summary>
        /// Describes the kind of folding range such as 'comment' or 'region'.
        /// </summary>
        /// <remarks>
        /// Used by commands such as 'fold all comments'.
        /// See <c>Constants.FoldingRangeKind</c>.
        /// </remarks>
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.FoldingRangeKind.AsStringEnum Kind { get; set; }

        /// <summary>
        /// The text that the client should show when the specified range is collapsed.
        /// </summary>
        [JsonPropertyName("collapsedText"), LspCompliant]
        public string CollapsedText { get; set; }
    }
}

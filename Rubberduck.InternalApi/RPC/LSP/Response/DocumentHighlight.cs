using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    /// <summary>
    /// A range inside a text document which deserves special attention.
    /// </summary>
    public class DocumentHighlight
    {
        /// <summary>
        /// The range this highlight applies to.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        /// <summary>
        /// The kind of highlight. See <c>Constants.DocumentHighlightKind</c>.
        /// </summary>
        [JsonPropertyName("kind")]
        public int Kind { get; set; }
    }
}

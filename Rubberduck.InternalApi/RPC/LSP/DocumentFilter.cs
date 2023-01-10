using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// A document filter denotes a document through properties like language, scheme or pattern.
    /// </summary>
    public class DocumentFilter
    {
        /// <summary>
        /// A language ID.
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; set; }

        /// <summary>
        /// A URI scheme, like 'file' or 'untitled'.
        /// </summary>
        [JsonPropertyName("scheme")]
        public string Scheme { get; set; }

        /// <summary>
        /// A glob pattern, like '*.{ts,js}'.
        /// </summary>
        [JsonPropertyName("pattern")]
        public string Pattern { get; set; }
    }
}

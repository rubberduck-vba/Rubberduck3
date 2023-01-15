using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DocumentFormattingParams : WorkDoneProgressParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The formatting options.
        /// </summary>
        [JsonPropertyName("options")]
        public FormattingOptions Options { get; set; }
    }

    public class RangeFormattingParams : DocumentFormattingParams
    {
        /// <summary>
        /// The range to format.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }
    }
}

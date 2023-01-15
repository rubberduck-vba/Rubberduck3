using System.Text.Json.Serialization;
namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{

    public interface ITextDocumentPositionParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument")]
        TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        [JsonPropertyName("position")]
        Position Position { get; set; }
    }

    /// <summary>
    /// A parameter literal used in requests to pass a text document and a position inside that document.
    /// </summary>

    public class TextDocumentPositionParams : ITextDocumentPositionParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        [JsonPropertyName("position")]
        public Position Position { get; set; }
    }
}

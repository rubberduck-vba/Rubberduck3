using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument
{
    public interface ITextDocumentPositionParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        [JsonPropertyName("position"), LspCompliant]
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
        [JsonPropertyName("textDocument"), LspCompliant]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        [JsonPropertyName("position"), LspCompliant]
        public Position Position { get; set; }
    }
}

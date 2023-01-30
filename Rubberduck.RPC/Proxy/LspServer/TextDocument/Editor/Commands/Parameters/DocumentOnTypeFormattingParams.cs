using Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Commands.Parameters
{
    public class DocumentOnTypeFormattingParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The caret position.
        /// </summary>
        [JsonPropertyName("position")]
        public Position Position { get; set; }

        /// <summary>
        /// The character that was typed.
        /// </summary>
        [JsonPropertyName("ch")]
        public string Ch { get; set; }

        /// <summary>
        /// The document formatting options.
        /// </summary>
        [JsonPropertyName("options")]
        public DocumentFormattingOptions Options { get; set; }
    }
}

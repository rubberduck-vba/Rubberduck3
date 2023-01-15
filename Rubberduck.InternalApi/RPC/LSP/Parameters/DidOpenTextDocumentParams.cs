using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DidOpenTextDocumentParams
    {
        /// <summary>
        /// The document that was opened.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentItem TextDocument { get; set; }
    }
}

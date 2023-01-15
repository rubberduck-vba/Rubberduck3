using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DidSaveTextDocumentParams
    {
        /// <summary>
        /// The document that was saved.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The saved content. <c>null</c> unless <c>IncludeText</c> was <c>true</c> when the save notification was requested.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}

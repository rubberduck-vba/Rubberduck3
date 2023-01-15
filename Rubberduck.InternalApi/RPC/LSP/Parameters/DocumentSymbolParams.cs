using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DocumentSymbolParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}

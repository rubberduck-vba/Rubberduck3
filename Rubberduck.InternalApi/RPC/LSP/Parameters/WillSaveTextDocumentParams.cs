using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class WillSaveTextDocumentParams
    {
        /// <summary>
        /// The document that will be saved.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The <c>TextDocumentSaveReason</c> code.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.TextDocumentSaveReason</c>.
        /// </remarks>
        [JsonPropertyName("reason")]
        public int Reason { get; set; }
    }
}

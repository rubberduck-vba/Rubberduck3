using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Commands.Parameters
{
    public class WillSaveTextDocumentParams
    {
        /// <summary>
        /// The document that will be saved.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The <c>TextDocumentSaveReason</c> code.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.TextDocumentSaveReason</c>.
        /// </remarks>
        [JsonPropertyName("reason"), LspCompliant]
        public Constants.TextDocumentSaveReason.AsEnum Reason { get; set; }
    }
}

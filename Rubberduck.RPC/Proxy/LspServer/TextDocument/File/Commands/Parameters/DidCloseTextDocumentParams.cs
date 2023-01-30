using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Commands.Parameters
{
    public class DidCloseTextDocumentParams
    {
        /// <summary>
        /// The document that was closed.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}

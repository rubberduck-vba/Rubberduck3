using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Commands.Parameters
{
    public class DidSaveTextDocumentParams
    {
        /// <summary>
        /// The document that was saved.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The saved content. <c>null</c> unless <c>IncludeText</c> was <c>true</c> when the save notification was requested.
        /// </summary>
        [JsonPropertyName("text"), LspCompliant]
        public string Text { get; set; }
    }
}

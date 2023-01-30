using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Commands.Parameters
{
    public class DocumentColorParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}

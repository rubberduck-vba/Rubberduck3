using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters
{
    public class SemanticTokensDeltaParams : WorkDoneProgressParams, IPartialResultParams
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

        /// <summary>
        /// The result ID of a previous response.
        /// </summary>
        /// <remarks>
        /// May point to a full or delta response, depending on what was received last.
        /// </remarks>
        [JsonPropertyName("previousResultId")]
        public string PreviousResultId { get; set; }
    }
}

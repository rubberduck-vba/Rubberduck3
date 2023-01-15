using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class CodeActionParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("context")]
        public CodeActionContext Context { get; set; }
    }
}

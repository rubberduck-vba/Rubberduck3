using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class CompletionParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken")]
        public string WorkDoneToken { get; set; }

        /// <summary>
        /// The completion context, if the client signaled support for it.
        /// </summary>
        [JsonPropertyName("context")]
        public CompletionContext Context { get; set; }
    }
}

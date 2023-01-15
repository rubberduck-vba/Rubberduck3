using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class WorkspaceDiagnosticsParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// The previous result IDs for the currently known diagnostic reports.
        /// </summary>
        [JsonPropertyName("previousResultIds")]
        public PreviousResultId[] PreviousResultIds { get; set; }
    }
}

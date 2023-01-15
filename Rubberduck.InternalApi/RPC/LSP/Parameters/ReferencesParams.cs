using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ReferencesParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
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

        [JsonPropertyName("context")]
        public ReferenceContext Context { get; set; }

        public class ReferenceContext
        {
            /// <summary>
            /// Whether the declaration of the symbol should be included in the results.
            /// </summary>
            [JsonPropertyName("includeDeclaration")]
            public bool IncludeDeclaration { get; set; }
        }
    }
}

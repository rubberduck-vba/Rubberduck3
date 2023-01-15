using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class WorkspaceSymbolParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// A query/search string to filter symbols by. Clients may send an empty string to request all symbols.
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; set; }
    }
}

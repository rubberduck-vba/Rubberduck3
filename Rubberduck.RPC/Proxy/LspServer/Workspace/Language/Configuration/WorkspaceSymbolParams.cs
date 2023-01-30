using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Configuration
{
    public class WorkspaceSymbolParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken"), LspCompliant]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// A query/search string to filter symbols by. Clients may send an empty string to request all symbols.
        /// </summary>
        [JsonPropertyName("query"), LspCompliant]
        public string Query { get; set; }
    }
}

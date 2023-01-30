using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Configuration
{
    public class WorkspaceDiagnosticsParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken"), LspCompliant]
        public string PartialResultToken { get; set; }

        [JsonPropertyName("identifier"), LspCompliant]
        public string Identifier { get; set; }

        /// <summary>
        /// The previous result IDs for the currently known diagnostic reports.
        /// </summary>
        [JsonPropertyName("previousResultIds"), LspCompliant]
        public PreviousResultId[] PreviousResultIds { get; set; }

        public class PreviousResultId
        {
            [JsonPropertyName("uri"), LspCompliant]
            public string DocumentUri { get; set; }

            [JsonPropertyName("value"), LspCompliant]
            public string Value { get; set; }
        }
    }
}

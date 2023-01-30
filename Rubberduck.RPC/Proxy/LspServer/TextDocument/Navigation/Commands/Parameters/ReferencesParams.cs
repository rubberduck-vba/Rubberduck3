using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Navigation.Commands.Parameters
{
    public class ReferencesParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken"), LspCompliant]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken"), LspCompliant]
        public string WorkDoneToken { get; set; }

        [JsonPropertyName("context"), LspCompliant]
        public ReferenceContext Context { get; set; }

        public class ReferenceContext
        {
            /// <summary>
            /// Whether the declaration of the symbol should be included in the results.
            /// </summary>
            [JsonPropertyName("includeDeclaration"), LspCompliant]
            public bool IncludeDeclaration { get; set; }
        }
    }
}

using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters
{
    public class CompletionParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
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

        /// <summary>
        /// The completion context, if the client signaled support for it.
        /// </summary>
        [JsonPropertyName("context"), LspCompliant]
        public CompletionContext Context { get; set; }
    }
}

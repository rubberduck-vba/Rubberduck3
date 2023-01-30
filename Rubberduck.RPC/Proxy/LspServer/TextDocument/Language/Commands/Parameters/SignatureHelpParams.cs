using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters
{
    public class SignatureHelpParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken")]
        public string WorkDoneToken { get; set; }

        [JsonPropertyName("context")]
        public SignatureHelpContext Context { get; set; }
    }
}

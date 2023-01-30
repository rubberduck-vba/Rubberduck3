using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Commands.Parameters
{
    public class DocumentFormattingParams : WorkDoneProgressParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The formatting options.
        /// </summary>
        [JsonPropertyName("options")]
        public DocumentFormattingOptions Options { get; set; }
    }

    public class RangeFormattingParams : DocumentFormattingParams
    {
        /// <summary>
        /// The range to format.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }
    }
}

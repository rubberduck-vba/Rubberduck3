using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Commands.Parameters
{
    public class ColorPresentationParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken"), LspCompliant]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The color information to request presentation for.
        /// </summary>
        [JsonPropertyName("color"), LspCompliant]
        public Color Color { get; set; }

        /// <summary>
        /// The range where the color would be inserted; serves as a context.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }
    }
}

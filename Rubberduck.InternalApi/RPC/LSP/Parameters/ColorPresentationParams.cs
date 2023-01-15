using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ColorPresentationParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The color information to request presentation for.
        /// </summary>
        [JsonPropertyName("color")]
        public Color Color { get; set; }

        /// <summary>
        /// The range where the color would be inserted; serves as a context.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }
    }
}

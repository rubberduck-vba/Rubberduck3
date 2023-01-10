using ProtoBuf;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "colorPresentationParams")]
    public class ColorPresentationParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        [ProtoMember(2)]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument")]
        [ProtoMember(3)]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The color information to request presentation for.
        /// </summary>
        [JsonPropertyName("color")]
        [ProtoMember(4)]
        public Color Color { get; set; }

        /// <summary>
        /// The range where the color would be inserted; serves as a context.
        /// </summary>
        [JsonPropertyName("range")]
        [ProtoMember(5)]
        public Range Range { get; set; }
    }
}

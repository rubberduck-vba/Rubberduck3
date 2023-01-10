using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "foldingRangeParams")]
    public class FoldingRangeParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        [ProtoMember(2)]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The document to provide folding ranges for.
        /// </summary>
        [JsonPropertyName("textDocument")]
        [ProtoMember(3)]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}

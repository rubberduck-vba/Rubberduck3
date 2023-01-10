using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "codeLensParams")]
    public class CodeLensParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        [ProtoMember(2)]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The document to provide links for.
        /// </summary>
        [JsonPropertyName("textDocument")]
        [ProtoMember(3)]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}

using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "codeActionParams")]
    public class CodeActionParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        [ProtoMember(2)]
        public string PartialResultToken { get; set; }

        [JsonPropertyName("textDocument")]
        [ProtoMember(3)]
        public TextDocumentIdentifier TextDocument { get; set; }

        [JsonPropertyName("range")]
        [ProtoMember(4)]
        public Range Range { get; set; }

        [JsonPropertyName("context")]
        [ProtoMember(5)]
        public CodeActionContext Context { get; set; }
    }
}

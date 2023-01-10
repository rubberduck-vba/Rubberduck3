using ProtoBuf;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "callHierarchyIncomingCallsParams")]
    public class CallHierarchyIncomingCallsParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [JsonPropertyName("partialResultToken")]
        [ProtoMember(2)]
        public string PartialResultToken { get; set; }

        [JsonPropertyName("item")]
        [ProtoMember(3)]
        public CallHierarchyItem Item { get; set; }
    }
}

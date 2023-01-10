using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "completionContext")]
    public class CompletionContext
    {
        [JsonPropertyName("triggerKind")]
        [ProtoMember(1)]
        public int TriggerKind { get; set; } = Constants.CompletionTriggerKind.Invoked;

        [JsonPropertyName("triggerCharacter")]
        [ProtoMember(2)]
        public string TriggerCharacter { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class CompletionContext
    {
        [JsonPropertyName("triggerKind")]
        public int TriggerKind { get; set; } = Constants.CompletionTriggerKind.Invoked;

        [JsonPropertyName("triggerCharacter")]
        public string TriggerCharacter { get; set; }
    }
}

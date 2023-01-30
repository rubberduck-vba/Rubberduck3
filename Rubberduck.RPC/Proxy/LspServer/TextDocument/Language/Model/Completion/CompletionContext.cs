using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CompletionContext
    {
        [JsonPropertyName("triggerKind"), LspCompliant]
        public Constants.CompletionItemKind.AsEnum TriggerKind { get; set; }

        [JsonPropertyName("triggerCharacter"), LspCompliant]
        public string TriggerCharacter { get; set; }
    }
}

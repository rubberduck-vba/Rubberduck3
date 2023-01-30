using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CompletionList
    {
        [JsonPropertyName("isIncomplete"), LspCompliant]
        public bool IsIncomplete { get; set; }

        [JsonPropertyName("items"), LspCompliant]
        public CompletionItem[] Items { get; set; }
    }
}

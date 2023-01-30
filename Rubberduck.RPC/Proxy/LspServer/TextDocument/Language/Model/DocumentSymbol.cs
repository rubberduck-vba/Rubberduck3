using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class DocumentSymbol
    {
        [JsonPropertyName("name"), LspCompliant]
        public string Name { get; set; }

        [JsonPropertyName("detail"), LspCompliant]
        public string Details { get; set; }

        [JsonPropertyName("kind"), LspCompliant]
        public Constants.SymbolKind.AsEnum SymbolKind { get; set; }

        [JsonPropertyName("tags"), LspCompliant]
        public Constants.SymbolTag.AsEnum[] SymbolTags { get; set; }

        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        [JsonPropertyName("selectionRange"), LspCompliant]
        public Range SelectionRange { get; set; }

        [JsonPropertyName("children"), LspCompliant]
        public DocumentSymbol[] Children { get; set; }
    }
}

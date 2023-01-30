using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model
{
    public class WorkspaceSymbol
    {
        [JsonPropertyName("name"), LspCompliant]
        public string Name { get; set; }

        [JsonPropertyName("kind"), LspCompliant]
        public Constants.SymbolKind.AsEnum SymbolKind { get; set; }

        [JsonPropertyName("tags"), LspCompliant]
        public Constants.SymbolTag.AsEnum[] SymbolTags { get; set; }

        [JsonPropertyName("containerName"), LspCompliant]
        public string Qualifier { get; set; }

        [JsonPropertyName("location"), LspCompliant]
        public Location Location { get; set; }

        [JsonPropertyName("data"), LspCompliant]
        public object Data { get; set; }
    }
}

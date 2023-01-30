using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class TypeHierarchyItem
    {
        [JsonPropertyName("name"), LspCompliant]
        public string Name { get; set; }

        [JsonPropertyName("kind"), LspCompliant]
        public Constants.SymbolKind.AsEnum SymbolKind { get; set; }

        [JsonPropertyName("tags"), LspCompliant]
        public Constants.SymbolTag.AsEnum[] SymbolTags { get; set; }

        /// <summary>
        /// More details for this item, e.g. the signature of a function.
        /// </summary>
        [JsonPropertyName("detail"), LspCompliant]
        public string Detail { get; set; }

        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }

        /// <summary>
        /// The range enclosing this symbol, not including leading/trailing whitespace but everything else e.g. comments and code.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
        /// </summary>
        /// <remarks>
        /// Must be contained by the <c>Range</c> specified.
        /// </remarks>
        [JsonPropertyName("selectionRange"), LspCompliant]
        public Range SelectionRange { get; set; }

        /// <summary>
        /// A data entry field that is preserved between a call hierarchy prepare and incoming/outgoing calls requests.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public object Data { get; set; }
    }
}

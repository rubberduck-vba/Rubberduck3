using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class CallHierarchyItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("kind")]
        public int SymbolKind { get; set; }

        [JsonPropertyName("tags")]
        public int[] SymbolTags { get; set; }

        /// <summary>
        /// More details for this item, e.g. the signature of a function.
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The range enclosing this symbol, not including leading/trailing whitespace but everything else e.g. comments and code.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        /// <summary>
        /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
        /// </summary>
        /// <remarks>
        /// Must be contained by the <c>Range</c> specified.
        /// </remarks>
        [JsonPropertyName("selectionRange")]
        public Range SelectionRange { get; set; }

        /// <summary>
        /// A data entry field that is preserved between a call hierarchy prepare and incoming/outgoing calls requests.
        /// </summary>
        [JsonPropertyName("data")]
        public LSPAny Data { get; set; }
    }
}

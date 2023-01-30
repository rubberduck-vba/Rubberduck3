using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Model
{
    public class SelectionRange
    {
        /// <summary>
        /// The <c>Range</c> of this selection range.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        /// <summary>
        /// The parent selection range, if any.
        /// </summary>
        [JsonPropertyName("parent")]
        public SelectionRange Parent { get; set; }
    }
}

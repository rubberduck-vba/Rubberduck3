using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
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

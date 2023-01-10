using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    /// <summary>
    /// The result of a hover request.
    /// </summary>
    public class Hover
    {
        /// <summary>
        /// The range this hover applies to.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        /// <summary>
        /// The markdown content of the hover popup.
        /// </summary>
        [JsonPropertyName("content")]
        public MarkupContent Content { get; set; }
    }
}

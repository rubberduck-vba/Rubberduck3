using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class MarkdownClientCapabilities
    {
        /// <summary>
        /// The name of the Markdown parser.
        /// </summary>
        [JsonPropertyName("parser")]
        public string Parser { get; set; }

        /// <summary>
        /// The parser version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// A list of HTML tags that the client allows / supports in Markdown.
        /// </summary>
        [JsonPropertyName("allowedTags")]
        public string[] AllowedHtmlTags { get; set; }
    }
}

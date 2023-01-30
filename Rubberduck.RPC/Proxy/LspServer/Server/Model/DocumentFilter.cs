using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Model
{
    /// <summary>
    /// A document filter denotes a document through properties like language, scheme or pattern.
    /// </summary>
    public class DocumentFilter
    {
        /// <summary>
        /// A language ID.
        /// </summary>
        [JsonPropertyName("language"), LspCompliant]
        public string Language { get; set; }

        /// <summary>
        /// A URI scheme, like 'file' or 'untitled'.
        /// </summary>
        [JsonPropertyName("scheme"), LspCompliant]
        public string Scheme { get; set; }

        /// <summary>
        /// A glob pattern, like '*.{ts,js}'.
        /// </summary>
        [JsonPropertyName("pattern"), LspCompliant]
        public string Pattern { get; set; }
    }
}

using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// Represents a location inside a resource, such as a line inside a text file.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The resource URI.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }
    }
}

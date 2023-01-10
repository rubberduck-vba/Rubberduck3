using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Represents a location inside a resource, such as a line inside a text file.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The resource URI.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }
    }
}

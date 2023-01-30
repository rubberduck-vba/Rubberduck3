using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// Represents a string content value that must be interpreted based on its <c>Kind</c>.
    /// </summary>
    public class MarkupContent
    {
        /// <summary>
        /// The type of content.
        /// </summary>
        /// <remarks>
        /// See <c>MarkupKind</c> constants.
        /// </remarks>
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.MarkupKind.AsStringEnum Kind { get; set; }

        /// <summary>
        /// The content itself.
        /// </summary>
        [JsonPropertyName("value"), LspCompliant]
        public string Value { get; set; }
    }
}

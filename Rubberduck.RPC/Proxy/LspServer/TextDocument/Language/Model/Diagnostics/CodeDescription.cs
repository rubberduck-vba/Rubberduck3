using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    /// <summary>
    /// Structure to capture a description for a diagnostic's error code.
    /// </summary>
    public class CodeDescription
    {
        /// <summary>
        /// An URI to open with more information about the diagnostic error.
        /// </summary>
        [JsonPropertyName("href"), LspCompliant]
        public string ReferenceUrl { get; set; }
    }
}

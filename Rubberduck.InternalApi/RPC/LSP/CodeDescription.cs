using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Structure to capture a description for an error code.
    /// </summary>
    public class CodeDescription
    {
        /// <summary>
        /// An URI to open with more information about the diagnostic error.
        /// </summary>
        [JsonPropertyName("href")]
        public string ReferenceUrl { get; set; }
    }
}

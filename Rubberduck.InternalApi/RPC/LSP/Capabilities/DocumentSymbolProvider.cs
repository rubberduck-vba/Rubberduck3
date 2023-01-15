using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DocumentSymbolProvider : WorkDoneProgressOptions
    {
        /// <summary>
        /// A human-readable string that is shown when multiple outline trees are shown for the same document.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}

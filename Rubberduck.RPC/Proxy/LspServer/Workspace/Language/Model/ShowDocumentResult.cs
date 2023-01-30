using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model
{
    public class ShowDocumentResult
    {
        /// <summary>
        /// Whether the document was successfully shown.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}

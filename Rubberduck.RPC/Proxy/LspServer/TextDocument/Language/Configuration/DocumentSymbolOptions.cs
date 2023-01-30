using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Configuration
{
    public class DocumentSymbolOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// A human-readable string that is shown when multiple outline trees are shown for the same document.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}

using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CompletionItemLabelDetails
    {
        /// <summary>
        /// An optional string rendered less prominently after the label, without any spacing.
        /// </summary>
        /// <remarks>
        /// Should be used for function signatures or type annotations.
        /// </remarks>
        [JsonPropertyName("detail"), LspCompliant]
        public string Detail { get; set; }

        /// <summary>
        /// An optional string rendered less prominently after the detail string (if any).
        /// </summary>
        /// <remarks>
        /// Should be used for fully qualified names or file paths.
        /// </remarks>
        [JsonPropertyName("description"), LspCompliant]
        public string Description { get; set; }
    }
}

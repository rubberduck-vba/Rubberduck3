using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class ColorPresentation
    {
        /// <summary>
        /// The label of this color presentation, shown e.g. on a color picker header.
        /// </summary>
        [JsonPropertyName("label"), LspCompliant]
        public string Label { get; set; }

        /// <summary>
        /// An edit applied to a document when selecting this presentation for the color.
        /// </summary>
        [JsonPropertyName("textEdit"), LspCompliant]
        public TextEdit TextEdit { get; set; }

        /// <summary>
        /// An optional array of edits applied when selecting this color presentation.
        /// </summary>
        [JsonPropertyName("additionalTextEdits"), LspCompliant]
        public TextEdit[] AdditionalTextEdits { get; set; }
    }
}

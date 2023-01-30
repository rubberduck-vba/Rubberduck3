using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SignatureInformation
    {
        [JsonPropertyName("label"), LspCompliant]
        public string Label { get; set; }

        [JsonPropertyName("documentation"), LspCompliant]
        public string Documentation { get; set; }

        [JsonPropertyName("parameters"), LspCompliant]
        public ParameterInformation[] Parameters { get; set; }

        /// <summary>
        /// The index (zero-based) of the active parameter.
        /// </summary>
        /// <remarks>
        /// If provided, this is used in place of 'SignatureHelp.ActiveParameter'.
        /// </remarks>
        [JsonPropertyName("activeParameter"), LspCompliant]
        public uint? ActiveParameter { get; set; }
    }
}

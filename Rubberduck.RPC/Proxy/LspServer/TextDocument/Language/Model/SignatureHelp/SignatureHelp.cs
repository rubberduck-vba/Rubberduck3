using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SignatureHelp
    {
        /// <summary>
        /// One or more signatures.
        /// </summary>
        [JsonPropertyName("signatures"), LspCompliant]
        public SignatureInformation[] Signatures { get; set; }

        /// <summary>
        /// The index (zero-based) of the active signature.
        /// </summary>
        [JsonPropertyName("activeSignature"), LspCompliant]
        public uint? ActiveSignature { get; set; }

        /// <summary>
        /// The index (zero-based) of the active parameter.
        /// </summary>
        [JsonPropertyName("activeParameter"), LspCompliant]
        public uint? ActiveParameter { get; set; }
    }
}

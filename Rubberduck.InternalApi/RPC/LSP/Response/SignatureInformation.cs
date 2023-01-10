using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class SignatureInformation
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("documentation")]
        public string Documentation { get; set; }

        [JsonPropertyName("parameters")]
        public ParameterInformation[] Parameters { get; set; }

        /// <summary>
        /// The index (zero-based) of the active parameter.
        /// </summary>
        /// <remarks>
        /// If provided, this is used in place of 'SignatureHelp.ActiveParameter'.
        /// </remarks>
        [JsonPropertyName("activeParameter")]
        public uint? ActiveParameter { get; set; }
    }
}

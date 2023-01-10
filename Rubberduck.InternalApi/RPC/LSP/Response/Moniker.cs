using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class Moniker
    {
        /// <summary>
        /// The scheme of the moniker.
        /// </summary>
        [JsonPropertyName("scheme")]
        public string Scheme { get; set; }

        /// <summary>
        /// The identifier of the moniker.
        /// </summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// The scope in which the moniker is unique.
        /// </summary>
        [JsonPropertyName("unique")]
        public string Unique { get; set; } = Constants.MonikerUniquenessLevel.Project;

        /// <summary>
        /// The moniker kind, if known.
        /// </summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = Constants.MonikerKind.Local;
    }
}

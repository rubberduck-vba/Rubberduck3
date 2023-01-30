using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class Moniker
    {
        /// <summary>
        /// The scheme of the moniker.
        /// </summary>
        [JsonPropertyName("scheme"), LspCompliant]
        public string Scheme { get; set; }

        /// <summary>
        /// The identifier of the moniker.
        /// </summary>
        [JsonPropertyName("identifier"), LspCompliant]
        public string Identifier { get; set; }

        /// <summary>
        /// The scope in which the moniker is unique.
        /// </summary>
        [JsonPropertyName("unique"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.MonikerUniquenessLevel.AsStringEnum Unique { get; set; }

        /// <summary>
        /// The moniker kind, if known.
        /// </summary>
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.MonikerKind.AsStringEnum Kind { get; set; }
    }
}

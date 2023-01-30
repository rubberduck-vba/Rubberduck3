using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Model
{
    public class LinkedEditingRanges
    {
        /// <summary>
        /// A list of ranges that can be renamed together. The ranges must have identical length and content, and cannot overlap.
        /// </summary>
        [JsonPropertyName("ranges"), LspCompliant]
        public Range[] Ranges { get; set; }

        /// <summary>
        /// An optional regex pattern that describes valid contents for the given range. If no pattern is provided, client configuration's word pattern will be used.
        /// </summary>
        [JsonPropertyName("wordPattern"), LspCompliant]
        public string WordPattern { get; set; }
    }
}

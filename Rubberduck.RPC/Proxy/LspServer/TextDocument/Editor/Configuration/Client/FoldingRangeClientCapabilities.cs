using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration.Client
{
    public class FoldingRangeClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// The maximum number of folding ranges that the client prefers to receive per document.
        /// </summary>
        /// <remarks>
        /// The value serves as a hint; servers are free to acknowledge this limit.
        /// </remarks>
        [JsonPropertyName("rangeLimit"), LspCompliant]
        public uint RangeLimit { get; set; }

        /// <summary>
        /// Signals that the client only supports folding complete lines.
        /// </summary>
        [JsonPropertyName("lineFoldingOnly"), LspCompliant]
        public bool LineFoldingOnly { get; set; } = true;

        /// <summary>
        /// Specific options for the folding range kind.
        /// </summary>
        [JsonPropertyName("foldingRangeKind"), LspCompliant]
        public FoldingRangeKinds FoldingRangeKind { get; set; }

        /// <summary>
        /// Specific options for the folding range.
        /// </summary>
        [JsonPropertyName("foldingRange"), LspCompliant]
        public FoldingRangeOptions FoldingRange { get; set; }

        public class FoldingRangeKinds
        {
            /// <summary>
            /// The folding range kinds supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
            public Constants.FoldingRangeKind.AsStringEnum[] ValueSet { get; set; }
        }

        public class FoldingRangeOptions
        {
            /// <summary>
            /// Signals that the client supports setting 'collapsedText' on folding ranges to display custom labels instead of the default text.
            /// </summary>
            [JsonPropertyName("collapsedText"), LspCompliant]
            public bool CollapsedText { get; set; }
        }
    }
}

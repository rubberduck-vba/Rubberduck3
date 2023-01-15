using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class FoldingRangeClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// The maximum number of folding ranges that the client prefers to receive per document.
        /// </summary>
        /// <remarks>
        /// The value serves as a hint; servers are free to acknowledge this limit.
        /// </remarks>
        [JsonPropertyName("rangeLimit")]
        public uint RangeLimit { get; set; }

        /// <summary>
        /// Signals that the client only supports folding complete lines.
        /// </summary>
        [JsonPropertyName("lineFoldingOnly")]
        public bool LineFoldingOnly { get; set; } = true;

        /// <summary>
        /// Specific options for the folding range kind.
        /// </summary>
        [JsonPropertyName("foldingRangeKind")]
        public FoldingRangeKinds FoldingRangeKind { get; set; }

        /// <summary>
        /// Specific options for the folding range.
        /// </summary>
        [JsonPropertyName("foldingRange")]
        public FoldingRangeOptions FoldingRange { get; set; }

        public class FoldingRangeKinds
        {
            /// <summary>
            /// The folding range kinds supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet")]
            public string[] ValueSet { get; set; }
        }

        public class FoldingRangeOptions
        {
            /// <summary>
            /// Signals that the client supports setting 'collapsedText' on folding ranges to display custom labels instead of the default text.
            /// </summary>
            [JsonPropertyName("collapsedText")]
            public bool CollapsedText { get; set; }
        }
    }
}

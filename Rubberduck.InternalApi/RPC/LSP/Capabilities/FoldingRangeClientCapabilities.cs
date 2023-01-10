using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "foldingRangeClientCapabilities")]
    public class FoldingRangeClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// The maximum number of folding ranges that the client prefers to receive per document.
        /// </summary>
        /// <remarks>
        /// The value serves as a hint; servers are free to acknowledge this limit.
        /// </remarks>
        [ProtoMember(2, Name = "rangeLimit")]
        public uint RangeLimit { get; set; }

        /// <summary>
        /// Signals that the client only supports folding complete lines.
        /// </summary>
        [ProtoMember(3, Name = "lineFoldingOnly")]
        public bool LineFoldingOnly { get; set; } = true;

        /// <summary>
        /// Specific options for the folding range kind.
        /// </summary>
        [ProtoMember(4, Name = "foldingRangeKind")]
        public FoldingRangeKinds FoldingRangeKind { get; set; }

        /// <summary>
        /// Specific options for the folding range.
        /// </summary>
        [ProtoMember(5, Name = "foldingRange")]
        public FoldingRangeOptions FoldingRange { get; set; }

        [ProtoContract(Name = "foldingRangeKinds")]
        public class FoldingRangeKinds
        {
            /// <summary>
            /// The folding range kinds supported by the client.
            /// </summary>
            [ProtoMember(1, Name = "valueSet")]
            public string[] ValueSet { get; set; }
        }

        [ProtoContract(Name = "foldingRangeOptions")]
        public class FoldingRangeOptions
        {
            /// <summary>
            /// Signals that the client supports setting 'collapsedText' on folding ranges to display custom labels instead of the default text.
            /// </summary>
            [ProtoMember(1, Name = "collapsedText")]
            public bool CollapsedText { get; set; }
        }
    }
}

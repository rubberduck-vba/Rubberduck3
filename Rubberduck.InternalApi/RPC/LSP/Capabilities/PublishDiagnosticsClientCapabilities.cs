using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "publishDiagnosticsClientCapabilities")]
    public class PublishDiagnosticsClientCapabilities
    {
        /// <summary>
        /// Whether the client accepts diagnostics with related information.
        /// </summary>
        [ProtoMember(1, Name = "relatedInformation")]
        public bool RelatedInformation { get; set; }

        /// <summary>
        /// Whether client supports the 'tag' property to provide metadata about a diagnostic.
        /// </summary>
        [ProtoMember(2, Name = "supportedTags")]
        public TagSupport SupportedTags { get; set; }

        [ProtoContract(Name = "tagSupport")]
        public class TagSupport
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [ProtoMember(1, Name = "valueSet")]
            public int[] ValueSet { get; set; }
        }

        /// <summary>
        /// Whether the client interprets the version property of the 'textDocument/publishDiagnostics' notification parameter.
        /// </summary>
        [ProtoMember(3, Name = "versionSupport")]
        public bool SupportsVersion { get; set; }

        /// <summary>
        /// Whether the client supports a 'codeDescription' property.
        /// </summary>
        [ProtoMember(4, Name = "codeDescriptionSupport")]
        public bool SupportsCodeDescription { get; set; }

        /// <summary>
        /// Whether code actions support the 'data' property (preserved between 'textDocument/publishDiagnostics' and 'textDocument/codeAction' requests).
        /// </summary>
        [ProtoMember(5, Name = "dataSupport")]
        public bool SupportsData { get; set; }
    }
}

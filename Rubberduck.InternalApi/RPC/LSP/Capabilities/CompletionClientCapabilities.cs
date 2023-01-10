using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "completionClientCapabilities")]
    public class CompletionClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for completion.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Client support for completion item specific capabilities.
        /// </summary>
        [ProtoMember(2, Name = "completionItem")]
        public CompletionItemCapabilities CompletionItem { get; set; }

        /// <summary>
        /// The <c>CompletionItemKind</c> values the client supports.
        /// </summary>
        [ProtoMember(3, Name = "completionItemKind")]
        public CompletionItemKindCapabilities CompletionItemKind { get; set; }

        /// <summary>
        /// <c>true</c> when the client supports additional context information in 'textDocument/completion' requests.
        /// </summary>
        [ProtoMember(4, Name = "contextSupport")]
        public bool SupportsContext { get; set; }

        /// <summary>
        /// The client's default when the completion item doesn't provide a <c>InsertTextMode</c> value.
        /// </summary>
        [ProtoMember(5, Name = "insertTextMode")]
        public int DefaultInsertTextMode { get; set; }

        /// <summary>
        /// The 'CompletionList' specific client capabilities.
        /// </summary>
        [ProtoMember(6, Name = "completionList")]
        public CompletionListCapabilities CompletionList { get; set; }

        [ProtoContract(Name = "completionListCapabilities")]
        public class CompletionListCapabilities
        {
            /// <summary>
            /// Client supports these default items on a completion list.
            /// </summary>
            /// <remarks>
            /// Lists the supported property names of the <c>CompletionList.ItemDefaults</c> object.
            /// </remarks>
            [ProtoMember(1, Name = "itemDefaults")]
            public string[] ItemDefaults { get; set; }
        }

        [ProtoContract(Name = "completionItemKindCapabilities")]
        public class CompletionItemKindCapabilities
        {
            /// <summary>
            /// The <c>Constants.CompletionItemKind</c> values supported by the client.
            /// </summary>
            [ProtoMember(1, Name = "valueSet")]
            public int[] ValueSet { get; set; }
        }

        [ProtoContract(Name = "completionItemCapabilities")]
        public class CompletionItemCapabilities
        {
            [ProtoMember(1, Name = "snippetSupport")]
            public bool SupportsSnippet { get; set; }

            [ProtoMember(2, Name = "commitCharacterSupport")]
            public bool SupportsCommitCharacter { get; set; }

            /// <remarks>
            /// See <c>Constants.MarkupKind</c>
            /// </remarks>
            [ProtoMember(3, Name = "documentationFormat")]
            public string[] DocumentationFormat { get; set; }

            [ProtoMember(4, Name = "deprecatedSupport")]
            public bool SupportsDeprecated { get; set; }

            [ProtoMember(5, Name = "preselectSupport")]
            public bool SupportsPreselect { get; set; }

            [ProtoMember(6, Name = "tagSupport")]
            public TagSupportCapabilities TagSupport { get; set; }

            [ProtoMember(7, Name = "insertReplaceSupport")]
            public bool SupportsInsertReplace { get; set; }

            [ProtoMember(8, Name = "resolveSupport")]
            public ResolveSupportCapabilities SupportsLazyResolution { get; set; }

            [ProtoMember(9, Name = "insertTextModeSupport")]
            public InsertTextModeSupportCapabilities SupportsInsertTextMode { get; set; }

            /// <summary>
            /// <c>true</c> when the client has support for completion item label details.
            /// </summary>
            [ProtoMember(10, Name = "labelDetailsSupport")]
            public bool SupportsLabelDetails { get; set; }

            [ProtoContract(Name = "tagSupportCapabilities")]
            public class TagSupportCapabilities
            {
                /// <summary>
                /// The tags supported by the client.
                /// </summary>
                [ProtoMember(1, Name = "valueSet")]
                public int[] ValueSet { get; set; }
            }

            [ProtoContract(Name = "resolveSupportCapabilities")]
            public class ResolveSupportCapabilities
            {
                /// <summary>
                /// The properties that a client can resolve lazily.
                /// </summary>
                [ProtoMember(1, Name = "properties")]
                public string[] Properties { get; set; }
            }

            [ProtoContract(Name = "insertTextModeSupportCapabilities")]
            public class InsertTextModeSupportCapabilities
            {
                [ProtoMember(1, Name = "valueSet")]
                public int[] ValueSet { get; set; }
            }
        }
    }
}



using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class CompletionClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for completion.
        /// </summary>
        [JsonPropertyName("dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Client support for completion item specific capabilities.
        /// </summary>
        [JsonPropertyName("completionItem")]
        public CompletionItemCapabilities CompletionItem { get; set; }

        /// <summary>
        /// The <c>CompletionItemKind</c> values the client supports.
        /// </summary>
        [JsonPropertyName("completionItemKind")]
        public CompletionItemKindCapabilities CompletionItemKind { get; set; }

        /// <summary>
        /// <c>true</c> when the client supports additional context information in 'textDocument/completion' requests.
        /// </summary>
        [JsonPropertyName("contextSupport")]
        public bool SupportsContext { get; set; }

        /// <summary>
        /// The client's default when the completion item doesn't provide a <c>InsertTextMode</c> value.
        /// </summary>
        [JsonPropertyName("insertTextMode")]
        public int DefaultInsertTextMode { get; set; }

        /// <summary>
        /// The 'CompletionList' specific client capabilities.
        /// </summary>
        [JsonPropertyName("completionList")]
        public CompletionListCapabilities CompletionList { get; set; }

        public class CompletionListCapabilities
        {
            /// <summary>
            /// Client supports these default items on a completion list.
            /// </summary>
            /// <remarks>
            /// Lists the supported property names of the <c>CompletionList.ItemDefaults</c> object.
            /// </remarks>
            [JsonPropertyName("itemDefaults")]
            public string[] ItemDefaults { get; set; }
        }

        public class CompletionItemKindCapabilities
        {
            /// <summary>
            /// The <c>Constants.CompletionItemKind</c> values supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet")]
            public int[] ValueSet { get; set; }
        }

        public class CompletionItemCapabilities
        {
            [JsonPropertyName("snippetSupport")]
            public bool SupportsSnippet { get; set; }

            [JsonPropertyName("commitCharacterSupport")]
            public bool SupportsCommitCharacter { get; set; }

            /// <remarks>
            /// See <c>Constants.MarkupKind</c>
            /// </remarks>
            [JsonPropertyName("documentationFormat")]
            public string[] DocumentationFormat { get; set; }

            [JsonPropertyName("deprecatedSupport")]
            public bool SupportsDeprecated { get; set; }

            [JsonPropertyName("preselectSupport")]
            public bool SupportsPreselect { get; set; }

            [JsonPropertyName("tagSupport")]
            public TagSupportCapabilities TagSupport { get; set; }

            [JsonPropertyName("insertReplaceSupport")]
            public bool SupportsInsertReplace { get; set; }

            [JsonPropertyName("resolveSupport")]
            public ResolveSupportCapabilities SupportsLazyResolution { get; set; }

            [JsonPropertyName("insertTextModeSupport")]
            public InsertTextModeSupportCapabilities SupportsInsertTextMode { get; set; }

            /// <summary>
            /// <c>true</c> when the client has support for completion item label details.
            /// </summary>
            [JsonPropertyName("labelDetailsSupport")]
            public bool SupportsLabelDetails { get; set; }

            public class TagSupportCapabilities
            {
                /// <summary>
                /// The tags supported by the client.
                /// </summary>
                [JsonPropertyName("valueSet")]
                public int[] ValueSet { get; set; }
            }

            public class ResolveSupportCapabilities
            {
                /// <summary>
                /// The properties that a client can resolve lazily.
                /// </summary>
                [JsonPropertyName("properties")]
                public string[] Properties { get; set; }
            }

            public class InsertTextModeSupportCapabilities
            {
                [JsonPropertyName("valueSet")]
                public int[] ValueSet { get; set; }
            }
        }
    }
}

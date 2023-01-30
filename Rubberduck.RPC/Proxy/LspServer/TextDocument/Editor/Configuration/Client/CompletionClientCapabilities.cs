using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration.Client
{
    public class CompletionClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration for completion.
        /// </summary>
        [JsonPropertyName("dynamicRegistration"), LspCompliant]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Client support for completion item specific capabilities.
        /// </summary>
        [JsonPropertyName("completionItem"), LspCompliant]
        public CompletionItemCapabilities CompletionItem { get; set; }

        /// <summary>
        /// The <c>CompletionItemKind</c> values the client supports.
        /// </summary>
        [JsonPropertyName("completionItemKind"), LspCompliant]
        public CompletionItemKindCapabilities CompletionItemKind { get; set; }

        /// <summary>
        /// <c>true</c> when the client supports additional context information in 'textDocument/completion' requests.
        /// </summary>
        [JsonPropertyName("contextSupport"), LspCompliant]
        public bool SupportsContext { get; set; }

        /// <summary>
        /// The client's default when the completion item doesn't provide a <c>InsertTextMode</c> value.
        /// </summary>
        [JsonPropertyName("insertTextMode"), LspCompliant]
        public int DefaultInsertTextMode { get; set; }

        /// <summary>
        /// The 'CompletionList' specific client capabilities.
        /// </summary>
        [JsonPropertyName("completionList"), LspCompliant]
        public CompletionListCapabilities CompletionList { get; set; }

        public class CompletionListCapabilities
        {
            /// <summary>
            /// Client supports these default items on a completion list.
            /// </summary>
            /// <remarks>
            /// Lists the supported property names of the <c>CompletionList.ItemDefaults</c> object.
            /// </remarks>
            [JsonPropertyName("itemDefaults"), LspCompliant]
            public string[] ItemDefaults { get; set; }
        }

        public class CompletionItemKindCapabilities
        {
            /// <summary>
            /// The <c>Constants.CompletionItemKind</c> values supported by the client.
            /// </summary>
            [JsonPropertyName("valueSet"), LspCompliant]
            public Constants.CompletionItemKind.AsEnum[] ValueSet { get; set; }
        }

        public class CompletionItemCapabilities
        {
            [JsonPropertyName("snippetSupport"), LspCompliant]
            public bool SupportsSnippet { get; set; }

            [JsonPropertyName("commitCharacterSupport"), LspCompliant]
            public bool SupportsCommitCharacter { get; set; }

            /// <remarks>
            /// See <c>Constants.MarkupKind</c>
            /// </remarks>
            [JsonPropertyName("documentationFormat"), LspCompliant]
            public string[] DocumentationFormat { get; set; }

            [JsonPropertyName("deprecatedSupport"), LspCompliant]
            public bool SupportsDeprecated { get; set; }

            [JsonPropertyName("preselectSupport"), LspCompliant]
            public bool SupportsPreselect { get; set; }

            [JsonPropertyName("tagSupport"), LspCompliant]
            public TagSupportCapabilities TagSupport { get; set; }

            [JsonPropertyName("insertReplaceSupport"), LspCompliant]
            public bool SupportsInsertReplace { get; set; }

            [JsonPropertyName("resolveSupport"), LspCompliant]
            public ResolveSupportCapabilities SupportsLazyResolution { get; set; }

            [JsonPropertyName("insertTextModeSupport"), LspCompliant]
            public InsertTextModeSupportCapabilities SupportsInsertTextMode { get; set; }

            /// <summary>
            /// <c>true</c> when the client has support for completion item label details.
            /// </summary>
            [JsonPropertyName("labelDetailsSupport"), LspCompliant]
            public bool SupportsLabelDetails { get; set; }

            public class TagSupportCapabilities
            {
                /// <summary>
                /// The tags supported by the client.
                /// </summary>
                [JsonPropertyName("valueSet"), LspCompliant]
                public Constants.CompletionItemTag.AsEnum[] ValueSet { get; set; }
            }

            public class ResolveSupportCapabilities
            {
                /// <summary>
                /// The properties that a client can resolve lazily.
                /// </summary>
                [JsonPropertyName("properties"), LspCompliant]
                public string[] Properties { get; set; }
            }

            public class InsertTextModeSupportCapabilities
            {
                [JsonPropertyName("valueSet"), LspCompliant]
                public Constants.InsertTextMode.AsEnum[] ValueSet { get; set; }
            }
        }
    }
}

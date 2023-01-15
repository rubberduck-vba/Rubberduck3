using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class CompletionOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The additional characters, beyond the defaults provided by the client, that should automatically trigger a completion request, for example ".".
        /// </summary>
        /// <remarks>
        /// Characters that make up identifiers do not need to be listed here.
        /// </remarks>
        [JsonPropertyName("triggerCharacters")]
        public string[] TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. Used as a fallback when individual completion items don't include commit characters.
        /// </summary>
        [JsonPropertyName("allCommitCharacters")]
        public string[] CommitCharacters { get; set; }

        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for a completion item.
        /// </summary>
        [JsonPropertyName("resolveProvider")]
        public bool IsResolveProvider { get; set; }

        /// <summary>
        /// Defines <c>CompletionItem</c> specific capabilities supported by the server.
        /// </summary>
        [JsonPropertyName("completionItem")]
        public CompletionItemCapabilities CompletionItem { get; set; }

        public class CompletionItemCapabilities
        {
            [JsonPropertyName("labelDetailsSupport")]
            public bool SupportsLabelDetails { get; set; }
        }
    }
}

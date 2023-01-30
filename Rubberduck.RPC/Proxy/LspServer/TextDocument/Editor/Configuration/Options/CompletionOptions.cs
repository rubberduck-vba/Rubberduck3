using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration
{
    public class CompletionOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The additional characters, beyond the defaults provided by the client, that should automatically trigger a completion request, for example ".".
        /// </summary>
        /// <remarks>
        /// Characters that make up identifiers do not need to be listed here.
        /// </remarks>
        [JsonPropertyName("triggerCharacters"), LspCompliant]
        public string[] TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. Used as a fallback when individual completion items don't include commit characters.
        /// </summary>
        [JsonPropertyName("allCommitCharacters"), LspCompliant]
        public string[] CommitCharacters { get; set; }

        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for a completion item.
        /// </summary>
        [JsonPropertyName("resolveProvider"), LspCompliant]
        public bool IsResolveProvider { get; set; }

        /// <summary>
        /// Defines <c>CompletionItem</c> specific capabilities supported by the server.
        /// </summary>
        [JsonPropertyName("completionItem"), LspCompliant]
        public CompletionItemCapabilities CompletionItem { get; set; }

        public class CompletionItemCapabilities
        {
            [JsonPropertyName("labelDetailsSupport"), LspCompliant]
            public bool SupportsLabelDetails { get; set; }
        }
    }
}

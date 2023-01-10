using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "completionOptions")]
    public class CompletionOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The additional characters, beyond the defaults provided by the client, that should automatically trigger a completion request, for example ".".
        /// </summary>
        /// <remarks>
        /// Characters that make up identifiers do not need to be listed here.
        /// </remarks>
        [ProtoMember(2, Name = "triggerCharacters")]
        public string[] TriggerCharacters { get; set; }

        /// <summary>
        /// The list of all possible characters that commit a completion. Used as a fallback when individual completion items don't include commit characters.
        /// </summary>
        [ProtoMember(3, Name = "allCommitCharacters")]
        public string[] CommitCharacters { get; set; }

        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for a completion item.
        /// </summary>
        [ProtoMember(4, Name = "resolveProvider")]
        public bool IsResolveProvider { get; set; }

        /// <summary>
        /// Defines <c>CompletionItem</c> specific capabilities supported by the server.
        /// </summary>
        [ProtoMember(5, Name = "completionItem")]
        public CompletionItemCapabilities CompletionItem { get; set; }

        [ProtoContract(Name = "completionItemCapabilities")]
        public class CompletionItemCapabilities
        {
            [ProtoMember(1, Name = "labelDetailsSupport")]
            public bool SupportsLabelDetails { get; set; }
        }
    }
}

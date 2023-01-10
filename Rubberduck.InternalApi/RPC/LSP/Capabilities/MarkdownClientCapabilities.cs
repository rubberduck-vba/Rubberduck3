using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "markdownClientCapabilities")]
    public class MarkdownClientCapabilities
    {
        /// <summary>
        /// The name of the Markdown parser.
        /// </summary>
        [ProtoMember(1, Name = "parser")]
        public string Parser { get; set; }

        /// <summary>
        /// The parser version.
        /// </summary>
        [ProtoMember(2, Name = "version")]
        public string Version { get; set; }

        /// <summary>
        /// A list of HTML tags that the client allows / supports in Markdown.
        /// </summary>
        [ProtoMember(3, Name = "allowedTags")]
        public string[] AllowedHtmlTags { get; set; }
    }
}

using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentLinkOptions")]
    public class DocumentLinkOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for document links.
        /// </summary>
        [ProtoMember(2, Name = "resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}

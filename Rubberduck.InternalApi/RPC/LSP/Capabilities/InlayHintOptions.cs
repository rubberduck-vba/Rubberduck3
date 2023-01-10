using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "inlayHintOptions")]
    public class InlayHintOptions : WorkDoneProgressOptions 
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for inlay hints.
        /// </summary>
        [ProtoMember(2, Name = "resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}

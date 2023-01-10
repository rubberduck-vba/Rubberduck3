using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "codeLensOptions")]
    public class CodeLensOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional CodeLens metadata.
        /// </summary>
        [ProtoMember(2, Name = "resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}

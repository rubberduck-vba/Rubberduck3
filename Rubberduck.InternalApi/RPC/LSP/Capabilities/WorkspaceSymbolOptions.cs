using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "workspaceSymbolOptions")]
    public class WorkspaceSymbolOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for workspace symbols.
        /// </summary>
        [ProtoMember(2, Name = "resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}

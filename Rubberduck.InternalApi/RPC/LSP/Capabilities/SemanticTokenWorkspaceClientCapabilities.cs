using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "semanticTokenWorkspaceClientCapabilities")]
    public class SemanticTokenWorkspaceClientCapabilities
    {
        /// <summary>
        /// Whether the client supports a refresh request sent from the server to the client.
        /// </summary>
        /// <remarks>
        /// This event is global and will force the client to refresh all semantic tokens currently shown. Use carefully.
        /// </remarks>
        [ProtoMember(1, Name = "refreshSupport")]
        public bool SupportsRefresh { get; set; }
    }
}

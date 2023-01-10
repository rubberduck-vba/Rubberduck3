using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "clientCapabilities")]
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace-specific client capabilities.
        /// </summary>
        [ProtoMember(1, Name = "workspace")]
        public WorkspaceClientCapabilities Workspace { get; set; }

        /// <summary>
        /// TextDocument-specific client capabilities.
        /// </summary>
        [ProtoMember(2, Name = "textDocument")]
        public TextDocumentClientCapabilities TextDocument { get; set; }

        // NotebookDocumentClientCapabilities

        /// <summary>
        /// Window-specific client capabilities.
        /// </summary>
        [ProtoMember(3, Name = "window")]
        public WindowClientCapabilities Window { get; set; }

        /// <summary>
        /// General client capabilities.
        /// </summary>
        [ProtoMember(4, Name = "general")]
        public GeneralClientCapabilities General { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        [ProtoMember(5, Name = "experimental")]
        public LSPAny Experimental { get; set; }
    }
}

using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didChangeWorkspaceFoldersParams")]
    public class DidChangeWorkspaceFoldersParams
    {
        /// <summary>
        /// Describes the workspace folders configuration changes.
        /// </summary>
        [ProtoMember(1, Name = "event")]
        public WorkspaceFoldersChangedEvent Event { get; set; }
    }
}

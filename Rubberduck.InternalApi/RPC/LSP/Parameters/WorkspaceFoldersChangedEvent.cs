using ProtoBuf;
using Rubberduck.InternalApi.RPC.LSP.Response;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "workspaceFoldersChangedEvent")]
    public class WorkspaceFoldersChangedEvent
    {
        /// <summary>
        /// An array containing the workspace folders that were added.
        /// </summary>
        [ProtoMember(1, Name = "added")]
        public WorkspaceFolder[] Added { get; set; }
        /// <summary>
        /// An array containing the workspace folders that were removed.
        /// </summary>
        [ProtoMember(2, Name = "removed")]
        public WorkspaceFolder[] Removed { get; set; }
    }
}

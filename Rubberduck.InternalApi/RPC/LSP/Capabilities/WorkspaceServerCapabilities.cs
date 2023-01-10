using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "workspaceServerCapabilities")]
    public class WorkspaceServerCapabilities
    {
        [ProtoMember(1, Name = "workspaceFolders")]
        public WorkspaceFoldersServerCapabilities WorkspaceFolders { get; set; }

        [ProtoMember(2, Name = "fileOperations")]
        public FileOperationServerCapabilities FileOperations { get; set; }
    }
}

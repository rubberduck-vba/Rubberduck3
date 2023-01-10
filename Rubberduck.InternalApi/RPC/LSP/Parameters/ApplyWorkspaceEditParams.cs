using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "applyWorkspaceEditParams")]
    public class ApplyWorkspaceEditParams
    {
        /// <summary>
        /// An optional label for the workspace edit, presented in the user interface e.g. to label the operation in an undo stack.
        /// </summary>
        [ProtoMember(1, Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// The edits to apply.
        /// </summary>
        [ProtoMember(2, Name = "edit")]
        public WorkspaceEdit Edit { get; set; }
    }
}

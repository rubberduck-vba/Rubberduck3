using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "workspaceEditClientCapabilities")]
    public class WorkspaceEditClientCapabilities
    {
        /// <summary>
        /// True if the client supports versioned document changes in workspace edits.
        /// </summary>
        [ProtoMember(1, Name = "documentChanges")]
        public bool DocumentChanges { get; set; }

        /// <summary>
        /// The resource operations the client supports. Clients should at least support 'create', 'rename', and 'delete' files and folders.
        /// </summary>
        /// <remarks>See <c>ResourceOperationKind</c> for the supported values.</remarks>
        [ProtoMember(2, Name = "resourceOperations")]
        public string[] ResourceOperations { get; set; } = new string[0];

        /// <summary>
        /// The failure handling strategy of a client when applying a workspace edit fails.
        /// </summary>
        /// <remarks>See <c>FailureHandlingKind</c> for the supported values.</remarks>
        [ProtoMember(3, Name = "failureHandling")]
        public string FailureHandlingStrategy { get; set; }

        /// <summary>
        /// Whether the client normalizes line endings to the client-specific newline character setting.
        /// </summary>
        [ProtoMember(4, Name = "normalizesLineEndings")]
        public bool NormalizesLineEndings { get; set; }

        /// <summary>
        /// Whether the client supports change annotations on text edits and create/rename/delete file/folder changes.
        /// </summary>
        [ProtoMember(5, Name = "changeAnnotationSupport")]
        public ChangeAnnotationSupport ChangeAnnotationSupport { get; set; }
    }

    [ProtoContract(Name = "changeAnnotationSupport")]
    public class ChangeAnnotationSupport
    {
        /// <summary>
        /// Whether the client groups edits with equal labels into tree nodes.
        /// </summary>
        [ProtoMember(1, Name = "groupsOnLabel")]
        public bool GroupsOnLabel { get; set; }
    }
}

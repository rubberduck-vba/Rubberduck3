using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class WorkspaceEditClientCapabilities
    {
        /// <summary>
        /// True if the client supports versioned document changes in workspace edits.
        /// </summary>
        [JsonPropertyName("documentChanges")]
        public bool DocumentChanges { get; set; }

        /// <summary>
        /// The resource operations the client supports. Clients should at least support 'create', 'rename', and 'delete' files and folders.
        /// </summary>
        /// <remarks>See <c>ResourceOperationKind</c> for the supported values.</remarks>
        [JsonPropertyName("resourceOperations")]
        public string[] ResourceOperations { get; set; } = new string[0];

        /// <summary>
        /// The failure handling strategy of a client when applying a workspace edit fails.
        /// </summary>
        /// <remarks>See <c>FailureHandlingKind</c> for the supported values.</remarks>
        [JsonPropertyName("failureHandling")]
        public string FailureHandlingStrategy { get; set; }

        /// <summary>
        /// Whether the client normalizes line endings to the client-specific newline character setting.
        /// </summary>
        [JsonPropertyName("normalizesLineEndings")]
        public bool NormalizesLineEndings { get; set; }

        /// <summary>
        /// Whether the client supports change annotations on text edits and create/rename/delete file/folder changes.
        /// </summary>
        [JsonPropertyName("changeAnnotationSupport")]
        public ChangeAnnotationSupport ChangeAnnotationSupport { get; set; }
    }

    public class ChangeAnnotationSupport
    {
        /// <summary>
        /// Whether the client groups edits with equal labels into tree nodes.
        /// </summary>
        [JsonPropertyName("groupsOnLabel")]
        public bool GroupsOnLabel { get; set; }
    }
}

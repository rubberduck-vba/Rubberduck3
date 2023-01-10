using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class RenameFile
    {
        /// <summary>
        /// <c>ResourceOperationKind.Rename</c>
        /// </summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = Constants.ResourceOperationKind.Rename;

        /// <summary>
        /// The old (existing) resource location.
        /// </summary>
        [JsonPropertyName("oldUri")]
        public string OldUri { get; set; }

        /// <summary>
        /// The new resource location.
        /// </summary>
        [JsonPropertyName("newUri")]
        public string NewUri { get; set; }

        /// <summary>
        /// File or folder renaming options.
        /// </summary>
        [JsonPropertyName("options")]
        public RenameFileOptions Options { get; set; } = RenameFileOptions.Default;
    }

    public class AnnotatedRenameFile : RenameFile
    {
        /// <summary>
        /// An annotation identifier describing the rename operation.
        /// </summary>
        [JsonPropertyName("annotationId")]
        public string AnnotationId { get; set; }
    }
}

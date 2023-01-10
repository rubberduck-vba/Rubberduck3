using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class DeleteFile
    {
        /// <summary>
        /// ResourceOperationKind.Delete
        /// </summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = Constants.ResourceOperationKind.Delete;

        /// <summary>
        /// The resource location to delete.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// file or folder deletion options.
        /// </summary>
        [JsonPropertyName("options")]
        public DeleteFileOptions Options { get; set; } = DeleteFileOptions.Default;
    }

    public class AnnotatedDeleteFile : DeleteFile
    {
        /// <summary>
        /// An annotation identifier describing the delete operation.
        /// </summary>
        [JsonPropertyName("annotationId")]
        public string AnnotationId { get; set; }
    }
}

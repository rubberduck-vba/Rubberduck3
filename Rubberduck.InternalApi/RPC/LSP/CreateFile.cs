using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class CreateFile
    {
        /// <summary>
        /// <c>ResourceOperationKind.Create</c>
        /// </summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = Constants.ResourceOperationKind.Create;

        /// <summary>
        /// The resource to create.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// File or folder creation options.
        /// </summary>
        [JsonPropertyName("options")]
        public CreateFileOptions Options { get; set; } = CreateFileOptions.Default;
    }

    public class AnnotatedCreateFile : CreateFile
    {
        /// <summary>
        /// An annotation identifier describing the create operation.
        /// </summary>
        [JsonPropertyName("annotationId")]
        public string AnnotationId { get; set; }
    }
}

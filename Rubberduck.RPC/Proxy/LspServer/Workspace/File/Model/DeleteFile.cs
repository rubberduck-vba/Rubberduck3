using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class DeleteFile
    {
        /// <summary>
        /// ResourceOperationKind.Delete
        /// </summary>
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.ResourceOperationKind.AsStringEnum Kind { get; set; }
            = Constants.ResourceOperationKind.AsStringEnum.Delete;

        /// <summary>
        /// The resource location to delete.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }

        /// <summary>
        /// file or folder deletion options.
        /// </summary>
        [JsonPropertyName("options"), LspCompliant]
        public DeleteFileOptions Options { get; set; }
    }

    public class AnnotatedDeleteFile : DeleteFile
    {
        /// <summary>
        /// An annotation identifier describing the delete operation.
        /// </summary>
        [JsonPropertyName("annotationId"), LspCompliant]
        public string AnnotationId { get; set; }
    }
}

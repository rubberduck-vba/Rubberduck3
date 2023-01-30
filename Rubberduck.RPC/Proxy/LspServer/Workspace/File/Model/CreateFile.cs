using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class CreateFile
    {
        /// <summary>
        /// <c>ResourceOperationKind.Create</c>
        /// </summary>
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.ResourceOperationKind.AsStringEnum Kind { get; set; } = Constants.ResourceOperationKind.AsStringEnum.Create;

        /// <summary>
        /// The resource to create.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
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
        [JsonPropertyName("annotationId"), LspCompliant]
        public string AnnotationId { get; set; }
    }
}

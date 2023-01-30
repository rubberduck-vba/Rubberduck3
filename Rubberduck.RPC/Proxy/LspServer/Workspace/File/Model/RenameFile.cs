using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class RenameFile
    {
        /// <summary>
        /// <c>ResourceOperationKind.Rename</c>
        /// </summary>
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.ResourceOperationKind.AsStringEnum Kind { get; set; } 
            = Constants.ResourceOperationKind.AsStringEnum.Rename;

        /// <summary>
        /// The old (existing) resource location.
        /// </summary>
        [JsonPropertyName("oldUri"), LspCompliant]
        public string OldUri { get; set; }

        /// <summary>
        /// The new resource location.
        /// </summary>
        [JsonPropertyName("newUri"), LspCompliant]
        public string NewUri { get; set; }

        /// <summary>
        /// File or folder renaming options.
        /// </summary>
        [JsonPropertyName("options"), LspCompliant]
        public RenameFileOptions Options { get; set; }
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

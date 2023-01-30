using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class WorkspaceEdit
    {
        /// <summary>
        /// Holds changes to existing resources.
        /// </summary>
        [JsonPropertyName("changes"), LspCompliant]
        public Dictionary<string, TextEdit> Changes { get; set; }

        /// <summary>
        /// An array of TextDocumentEdit, CreateFile, RenameFile, and/or DeleteFile objects.
        /// </summary>
        [JsonPropertyName("documentChanges"), LspCompliant]
        public object[] DocumentChanges { get; set; }

        /// <summary>
        /// A map of change annotations that can be referenced in AnnotatedTextEdits or create, rename, and delete file/folder operations.
        /// </summary>
        [JsonPropertyName("changeAnnotations"), LspCompliant]
        public Dictionary<string, ChangeAnnotation> ChangeAnnotations { get; set; }
    }
}

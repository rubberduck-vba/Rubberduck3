using System.Collections.Generic;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class WorkspaceEdit
    {
        /// <summary>
        /// Holds changes to existing resources.
        /// </summary>
        public Dictionary<string, TextEdit> Changes { get; set; } // TODO verify how dictionary serializes into json
        
        /// <summary>
        /// An array of TextDocumentEdit, CreateFile, RenameFile, and/or DeleteFile objects.
        /// </summary>
        public object[] DocumentChanges { get; set; }

        /// <summary>
        /// A map of change annotations that can be referenced in AnnotatedTextEdits or create, rename, and delete file/folder operations.
        /// </summary>
        public Dictionary<string, ChangeAnnotation> ChangeAnnotations { get; set; }
    }
}

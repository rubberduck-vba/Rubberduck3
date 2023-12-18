using Rubberduck.UI.Shell.Document;
using System;

namespace Rubberduck.Editor.Shell.Document.Tabs
{
    /// <summary>
    /// The base view model for a type of document tab that contains code managed by a language server.
    /// </summary>
    public abstract class CodeDocumentTabViewModel : DocumentTabViewModel
    {
        public CodeDocumentTabViewModel(Uri documentUri, string language, string title, string content, bool isReadOnly)
            : base(documentUri, language, title, content, isReadOnly)
        {
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.SourceFile;
    }
}

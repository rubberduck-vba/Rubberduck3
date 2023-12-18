using Rubberduck.UI.Shell.Document;
using System;

namespace Rubberduck.Editor.Shell.Document.Tabs
{
    /// <summary>
    /// A view model for a type of document tab that contains a markdown document.
    /// </summary>
    public class MarkdownDocumentTabViewModel : DocumentTabViewModel
    {
        public MarkdownDocumentTabViewModel(Uri documentUri, string title, string content, bool isReadOnly = true)
            : base(documentUri, "md/html", title, content, isReadOnly)
        {
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.MarkdownDocument;
    }
}

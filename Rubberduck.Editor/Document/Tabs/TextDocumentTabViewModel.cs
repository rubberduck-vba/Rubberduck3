using System;

namespace Rubberduck.Editor.Document.Tabs
{
    /// <summary>
    /// A view model for a type of document tab that contains a plain text document.
    /// </summary>
    public class TextDocumentTabViewModel : DocumentTabViewModel
    {
        public TextDocumentTabViewModel(Uri documentUri, string title, string content, bool isReadOnly = false)
            : base(documentUri, "text/plain", title, content, isReadOnly)
        {
        }
    }
}

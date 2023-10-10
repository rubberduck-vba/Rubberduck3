using System;

namespace Rubberduck.UI.RubberduckEditor.Document
{
    public class MarkdownDocumentTabViewModel : DocumentTabViewModel
    {
        public MarkdownDocumentTabViewModel(Uri documentUri, string title, string content)
            : base(documentUri, title, content)
        {
        }
    }
}

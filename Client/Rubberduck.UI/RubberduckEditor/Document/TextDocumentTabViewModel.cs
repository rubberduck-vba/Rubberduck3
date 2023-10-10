using System;

namespace Rubberduck.UI.RubberduckEditor.Document
{
    public class TextDocumentTabViewModel : DocumentTabViewModel
    {
        public TextDocumentTabViewModel(Uri documentUri, string title, string content)
            : base(documentUri, title, content)
        {
        }
    }
}

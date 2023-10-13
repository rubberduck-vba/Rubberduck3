using System;

namespace Rubberduck.Editor.Document.Tabs
{
    public class TextDocumentTabViewModel : DocumentTabViewModel
    {
        public TextDocumentTabViewModel(Uri documentUri, string title, string content)
            : base(documentUri, title, content)
        {
        }
    }
}

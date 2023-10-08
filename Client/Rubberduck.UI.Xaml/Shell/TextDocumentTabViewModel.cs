using System;

namespace Rubberduck.UI.Xaml.Shell
{
    public class TextDocumentTabViewModel : DocumentTabViewModel
    {
        public TextDocumentTabViewModel(Uri documentUri, string title, string content)
            : base(documentUri, title, content)
        {
        }
    }
}

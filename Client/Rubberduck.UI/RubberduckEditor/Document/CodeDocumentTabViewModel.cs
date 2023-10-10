using System;

namespace Rubberduck.UI.RubberduckEditor.Document
{
    public class CodeDocumentTabViewModel : DocumentTabViewModel
    {
        public CodeDocumentTabViewModel(Uri documentUri, string title, string content) 
            : base(documentUri, title, content)
        {
        }
    }
}

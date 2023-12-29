using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using System;

namespace Rubberduck.Editor.Shell.Document.Tabs
{
    /// <summary>
    /// A view model for a type of document tab that contains a plain text document.
    /// </summary>
    public class TextDocumentTabViewModel : DocumentTabViewModel
    {
        public TextDocumentTabViewModel(Uri documentUri, string title, string content, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
            : base(documentUri, "text/plain", title, content, isReadOnly, showSettingsCommand, closeToolWindowCommand)
        {
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.TextDocument;
    }
}

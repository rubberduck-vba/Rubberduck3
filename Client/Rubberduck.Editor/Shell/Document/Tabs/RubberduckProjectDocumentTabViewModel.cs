using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using System;

namespace Rubberduck.Editor.Shell.Document.Tabs
{
    /// <summary>
    /// A view model for a type of document tab that contains a Rubberduck Project (.rdproj) file.
    /// </summary>
    public class RubberduckProjectDocumentTabViewModel : DocumentTabViewModel
    {
        public RubberduckProjectDocumentTabViewModel(Uri documentUri, string title, string content, bool isReadOnly, 
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
            : base(documentUri, "json", title, content, isReadOnly,
                  showSettingsCommand, closeToolWindowCommand)
        {
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.ProjectFile;
    }
}

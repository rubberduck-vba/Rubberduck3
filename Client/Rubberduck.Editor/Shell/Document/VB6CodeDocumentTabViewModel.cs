using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.StatusBar;
using System;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// A view model for a type of document tab that contains code for a Visual Basic 6.0 module/component.
    /// </summary>
    public class VB6CodeDocumentTabViewModel : CodeDocumentTabViewModel
    {
        public VB6CodeDocumentTabViewModel(Uri documentUri, string title, string content, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus)
            : base(documentUri, SupportedLanguage.VB6.Id, title, content, isReadOnly, showSettingsCommand, closeToolWindowCommand, activeDocumentStatus)
        {
        }
    }
}

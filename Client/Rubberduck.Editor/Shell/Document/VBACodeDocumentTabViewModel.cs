using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.StatusBar;
using System;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// A view model for a type of document tab that contains code for a Visual Basic for Applications 7.0 module/component.
    /// </summary>
    public class VBACodeDocumentTabViewModel : CodeDocumentTabViewModel
    {
        public VBACodeDocumentTabViewModel(WorkspaceUri documentUri, string title, string content, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus,
            Func<ILanguageClient> lsp)
            : base(documentUri, SupportedLanguage.VBA.Id, title, content, isReadOnly, showSettingsCommand, closeToolWindowCommand, activeDocumentStatus, lsp)
        {
        }
    }
}

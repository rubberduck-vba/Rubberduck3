using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// A view model for a type of document tab that contains a Rubberduck Project (.rdproj) file.
    /// </summary>
    public class RubberduckProjectDocumentTabViewModel : DocumentTabViewModel
    {
        public RubberduckProjectDocumentTabViewModel(WorkspaceUri documentUri, string title, string content, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus,
            Func<ILanguageClient> lsp)
            : base(documentUri, "json", title, content, isReadOnly,
                  showSettingsCommand, closeToolWindowCommand, activeDocumentStatus, lsp)
        {
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.ProjectFile;
    }
}

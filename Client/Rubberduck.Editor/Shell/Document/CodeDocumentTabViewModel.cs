using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using System;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// The base view model for a type of document tab that contains code managed by a language server.
    /// </summary>
    public abstract class CodeDocumentTabViewModel : DocumentTabViewModel
    {
        public CodeDocumentTabViewModel(WorkspaceUri documentUri, string language, string title, string content, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus,
            Func<ILanguageClient> lsp)
            : base(documentUri, language, title, content, isReadOnly, showSettingsCommand, closeToolWindowCommand, activeDocumentStatus, lsp)
        {
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.SourceFile;
    }
}

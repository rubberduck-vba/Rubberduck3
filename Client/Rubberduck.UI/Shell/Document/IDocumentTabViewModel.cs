using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.UI.Shell.Document
{
    public enum SupportedDocumentType
    {
        /// <summary>
        /// A plain text document, with encoding options.
        /// </summary>
        TextDocument,
        /// <summary>
        /// A markdown-formatted text document.
        /// </summary>
        MarkdownDocument,
        /// <summary>
        /// A JSON specification for a Rubberduck project.
        /// </summary>
        ProjectFile,
        /// <summary>
        /// A text file that gets synchronized with the VBE as part of the host's corresponding VBA project.
        /// </summary>
        SourceFile,
    }

    public interface IDocumentTabViewModel : ITabViewModel
    {
        WorkspaceUri DocumentUri { get; set; }
        DocumentState DocumentState { get; }
        string Language { get; set; }
        bool IsReadOnly { get; set; }

        void NotifyDocumentChanged();

        SupportedDocumentType DocumentType { get; }
        IDocumentStatusViewModel Status { get; }

        Task<IEnumerable<FoldingRange>> RequestFoldingsAsync();
        Task<IEnumerable<Diagnostic>> RequestDiagnosticsAsync();
    }
}

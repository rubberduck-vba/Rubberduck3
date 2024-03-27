using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;
using System;

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
        Uri DocumentUri { get; set; }
        DocumentState DocumentState { get; set; }

        bool IsReadOnly { get; set; }

        SupportedDocumentType DocumentType { get; }
        IDocumentStatusViewModel Status { get; }
    }

    public interface ICodeDocumentTabViewModel : IDocumentTabViewModel
    {
        event EventHandler CodeDocumentStateChanged;
        string LanguageId { get; }

        WorkspaceFileUri CodeDocumentUri { get; set; }
        CodeDocumentState CodeDocumentState { get; set; }
    }
}

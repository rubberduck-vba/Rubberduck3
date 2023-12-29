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
        public Uri DocumentUri { get; set; }
        public string Language { get; set; }
        public bool IsReadOnly { get; set; }

        public SupportedDocumentType DocumentType { get; }
    }
}

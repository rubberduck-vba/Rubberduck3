using Rubberduck.UI.Shell.Document;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// An interface representing a document window status bar.
    /// </summary>
    public interface IDocumentStatusViewModel : IProgressStatusViewModel
    {
        SupportedDocumentType DocumentType { get; set; }
        string DocumentName { get; set; }
        bool IsReadOnly { get; set; }

        /// <summary>
        /// The number of lines in the document.
        /// </summary>
        int DocumentLines { get; set; }
        /// <summary>
        /// The number of characters in the document.
        /// </summary>
        int DocumentLength { get; set; }
        /// <summary>
        /// The current caret position in the document.
        /// </summary>
        int CaretOffset { get; set; }
        /// <summary>
        /// The current line position of the selection caret.
        /// </summary>
        int CaretLine { get; set; }
        /// <summary>
        /// The current column position of the selection caret.
        /// </summary>
        int CaretColumn { get; set; }
    }
}

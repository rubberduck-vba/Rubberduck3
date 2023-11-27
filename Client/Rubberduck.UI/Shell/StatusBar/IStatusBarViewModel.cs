namespace Rubberduck.UI.Shell.StatusBar
{
    public interface IStatusBarViewModel
    {
        /// <summary>
        /// Gets/sets the number of lines in the current document.
        /// </summary>
        int DocumentLines { get; set; }
        /// <summary>
        /// Gets/sets the number of characters in the current document.
        /// </summary>
        int DocumentLength { get; set; }
        /// <summary>
        /// Gets/sets the current offset of the caret position.
        /// </summary>
        int CaretOffset { get; set; }
        /// <summary>
        /// Gets/sets the current line of the caret position.
        /// </summary>
        int CaretLine { get; set; }
        /// <summary>
        /// Gets/sets the current column of the caret position.
        /// </summary>
        int CaretColumn { get; set; }
        /*
        /// <summary>
        /// Gets/sets the current qualified name at the caret position.
        /// </summary>
        QualifiedMemberName CodeLocation { get; set; }
        */
        /// <summary>
        /// Gets/sets the number of issues (parsing/syntax errors and static code analysis) in the current editor pane.
        /// </summary>
        int IssuesCount { get; set; }
        /// <summary>
        /// Gets/sets the current server state.
        /// </summary>
        string ServerStateText { get; set; }

        ServerConnectionState ServerConnectionState { get; set; }
        /// <summary>
        /// Gets/sets the current progress indicator value.
        /// </summary>
        int ProgressValue { get; set; }
        /// <summary>
        /// Gets/sets the progress indicator max value.
        /// </summary>
        int ProgressMaxValue { get; set; }

        bool ShowDocumentStatusItems { get; set; }
    }
}

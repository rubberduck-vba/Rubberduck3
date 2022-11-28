namespace Rubberduck.UI.Abstract
{
    public interface IStatusUpdate
    {
        string CurrentStatus { get; }
        void UpdateStatus(string status);
    }

    public interface IEditorStatusViewModel
    {
        /// <summary>
        /// Gets/sets the current line and column of the caret position.
        /// </summary>
        string CaretLocation { get; set; }
        /// <summary>
        /// Gets/sets the current qualified name at the caret position.
        /// </summary>
        string CodeLocation { get; set; }
        /// <summary>
        /// Gets/sets the number of issues (parsing/syntax errors and static code analysis) in the current editor pane.
        /// </summary>
        int IssuesCount { get; set; }
        /// <summary>
        /// Gets/sets the current parser state.
        /// </summary>
        string ParserState { get; set; }
        /// <summary>
        /// Gets/sets the current progress indicator value.
        /// </summary>
        int ProgressValue { get; set; }
        /// <summary>
        /// Gets/sets the progress indicator max value.
        /// </summary>
        int ProgressMaxValue { get; set; }
    }
}

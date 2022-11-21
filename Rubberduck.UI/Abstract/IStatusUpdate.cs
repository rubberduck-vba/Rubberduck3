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
        /// Current line and column of the caret position.
        /// </summary>
        string CaretLocation { get; set; }
        /// <summary>
        /// The number of issues (parsing/syntax errors and static code analysis) in the current module.
        /// </summary>
        int IssuesCount { get; set; }
    }
}

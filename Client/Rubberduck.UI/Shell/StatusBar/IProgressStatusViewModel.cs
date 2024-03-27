using System.Windows.Input;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// The base interface for a status bar view model that can display progress updates.
    /// </summary>
    public interface IProgressStatusViewModel : IStatusBarViewModel
    {
        /// <summary>
        /// An indicator, <c>true</c> whenever the document's idle timer is ticking, indicating the user is typing in the editor.
        /// </summary>
        bool IsWriting { get; set; }

        /// <summary>
        /// Gets or sets the current progress indicator value.
        /// </summary>
        int ProgressValue { get; set; }
        /// <summary>
        /// Gets or sets the progress indicator max value.
        /// </summary>
        int ProgressMaxValue { get; set; }

        string? ProgressMessage { get; set; }

        bool CanCancelWorkDoneProgress { get; set; }
        ICommand CancelWorkDoneProgressCommand { get; set; }
    }
}

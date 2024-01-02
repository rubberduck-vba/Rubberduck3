using System.Windows.Input;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// The base interface for a status bar view model that can display progress updates.
    /// </summary>
    public interface IProgressStatusViewModel : IStatusBarViewModel
    {
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

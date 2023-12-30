using System.Windows.Input;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// A status bar view model that shows language server connection status.
    /// </summary>
    public interface ILanguageServerStatusViewModel : IStatusBarViewModel
    {
        /// <summary>
        /// Gets or sets the server connection state.
        /// </summary>
        bool IsConnected { get; set; }

        ICommand ShowLanguageServerTraceCommand { get; set; }
    }
}

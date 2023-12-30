using System.Collections;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// The base interface for a status bar view model.
    /// </summary>
    public interface IStatusBarViewModel
    {
        /// <summary>
        /// Gets/sets the current server state.
        /// </summary>
        string StatusText { get; set; }
    }
}

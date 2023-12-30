using System.Collections.Generic;

namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// A status bar view model that shows notifications.
    /// </summary>
    public interface INotificationStatusViewModel : IStatusBarViewModel
    {
        ICollection<INotificationViewModel> Notifications { get; }
    }
}

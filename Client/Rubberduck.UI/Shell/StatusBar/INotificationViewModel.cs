using Rubberduck.UI.Message;

namespace Rubberduck.UI.Shell.StatusBar
{
    public interface INotificationViewModel
    {
        string Title { get; set; }
        string Description { get; set; }
        MessageAction[] Actions { get; } 
    }
}

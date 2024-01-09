using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Message;

namespace Rubberduck.UI.Windows
{
    public interface IDialogWindowViewModel
    {
        string Title { get; }
        MessageActionCommand[] Actions { get; }
        MessageAction? SelectedAction { get; set; }
        bool IsEnabled { get; set; }
    }
}

using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;

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

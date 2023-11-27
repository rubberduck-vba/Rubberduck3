using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;

namespace Rubberduck.UI.Windows
{
    public interface IDialogWindowViewModel : IWindowViewModel
    {
        MessageActionCommand[] Actions { get; }
        MessageAction? SelectedAction { get; set; }
        bool IsEnabled { get; set; }
    }
}

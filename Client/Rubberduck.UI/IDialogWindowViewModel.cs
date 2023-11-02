using Rubberduck.UI.Command;

namespace Rubberduck.UI
{
    public interface IDialogWindowViewModel : IWindowViewModel
    {
        MessageActionCommand[] Actions { get; }
        MessageAction? SelectedAction { get; set; }
        bool IsEnabled { get; set; }
    }
}

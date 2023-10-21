using Rubberduck.Editor.Command;
using System.ComponentModel;

namespace Rubberduck.Editor.Message
{
    public interface IWindowViewModel : INotifyPropertyChanged
    {
        string Title { get; }
    }

    public interface IDialogWindowViewModel : IWindowViewModel
    {
        MessageActionCommand[] Actions { get; }
        MessageAction? SelectedAction { get; set; }
        bool IsEnabled { get; set; }
    }

    public interface IMessageWindowViewModel : IDialogWindowViewModel
    {
        string Message { get; }
        string? Verbose { get; }
    }
}

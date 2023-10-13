using System.ComponentModel;

namespace Rubberduck.Editor.Message
{
    public interface IMessageWindowViewModel : INotifyPropertyChanged
    {
        string Title { get; }
        string Message { get; }
        string? Verbose { get; }

        MessageAction[] Actions { get; }
        MessageAction? SelectedAction { get; set; }
    }
}

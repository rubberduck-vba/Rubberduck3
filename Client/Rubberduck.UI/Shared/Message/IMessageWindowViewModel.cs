using Rubberduck.UI.Windows;

namespace Rubberduck.UI.Shared.Message
{
    public interface IMessageWindowViewModel : IDialogWindowViewModel
    {
        string Message { get; }
        string? Verbose { get; }
    }
}

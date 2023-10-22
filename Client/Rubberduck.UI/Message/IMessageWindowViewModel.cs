using Rubberduck.UI;

namespace Rubberduck.UI.Message
{
    public interface IMessageWindowViewModel : IDialogWindowViewModel
    {
        string Message { get; }
        string? Verbose { get; }
    }
}

using Rubberduck.UI.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.LanguageServerTrace
{
    public interface IServerTraceViewModel : IToolWindowViewModel
    {
        ICommand CopyContentCommand { get; }
        ICommand ClearContentCommand { get; }
        ICommand OpenLogFileCommand { get; }
        bool IsPaused { get; set; }
        string TextContent { get; set; }
    }
}

using Rubberduck.UI.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Shell.Tools.ServerTrace
{
    public interface IServerTraceViewModel : IToolWindowViewModel
    {
        ICommand CopyContentCommand { get; }
        ICommand ClearContentCommand { get; }
        ICommand OpenLogFileCommand { get; }
        bool IsPaused { get; set; }

        void OnServerTrace(string message, string? verbose);
    }
}

using System.Windows.Input;

namespace Rubberduck.UI.Shell.Tools.ServerTrace
{
    public static class ServerTraceCommands
    {
        public static RoutedCommand PauseTraceCommand { get; }
            = new RoutedCommand(nameof(PauseTraceCommand), typeof(ServerTraceControl));

        public static RoutedCommand ClearContentCommand { get; }
            = new RoutedCommand(nameof(ClearContentCommand), typeof(ServerTraceControl));

        public static RoutedCommand CopyContentCommand { get; }
            = new RoutedCommand(nameof(CopyContentCommand), typeof(ServerTraceControl));

        public static RoutedCommand OpenLogFileCommand { get; }
            = new RoutedCommand(nameof(OpenLogFileCommand), typeof(ServerTraceControl));
    }
}

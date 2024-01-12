using System.Windows.Input;
using Resx = Rubberduck.Resources.v3.RubberduckUICommands;

namespace Rubberduck.UI.Shell.Tools.ServerTrace
{
    public static class ServerTraceCommands
    {
        public static RoutedUICommand PauseTraceCommand { get; }
            = new RoutedUICommand(Resx.ServerTraceCommands_PauseTraceCommandText, nameof(PauseTraceCommand), typeof(ServerTraceControl));

        public static RoutedUICommand ClearContentCommand { get; }
            = new RoutedUICommand(Resx.ServerTraceCommands_ClearContentCommandText, nameof(ClearContentCommand), typeof(ServerTraceControl));

        public static RoutedUICommand CopyContentCommand { get; }
            = new RoutedUICommand(Resx.ServerTraceCommands_CopyContentCommandText, nameof(CopyContentCommand), typeof(ServerTraceControl));

        public static RoutedUICommand OpenLogFileCommand { get; }
            = new RoutedUICommand(Resx.ServerTraceCommands_OpenLogFileCommandText, nameof(OpenLogFileCommand), typeof(ServerTraceControl));

        public static RoutedUICommand ShutdownServerCommand { get; }
            = new RoutedUICommand(Resx.ServerTraceCommands_ShutdownServerCommandText, nameof(ShutdownServerCommand), typeof(ServerTraceControl));
    }
}

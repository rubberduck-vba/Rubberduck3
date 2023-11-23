using System.Windows.Input;

namespace Rubberduck.UI.Command
{
    public static class AnalyzeCommands
    {
        public static RoutedCommand ShowDiagnosticsCommand { get; } = ViewCommands.ShowDiagnosticsCommand;
        public static RoutedCommand ShowCodeMetricsCommand { get; } = ViewCommands.ShowCodeMetricsCommand;
    }
}

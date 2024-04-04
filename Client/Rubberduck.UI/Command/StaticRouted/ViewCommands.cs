using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command.StaticRouted
{
    public static class ViewCommands
    {
        public static RoutedCommand ViewCodeCommand { get; }
            = new RoutedCommand(nameof(ViewCodeCommand), typeof(Window));
        public static RoutedCommand ViewDesignerCommand { get; }
            = new RoutedCommand(nameof(ViewDesignerCommand), typeof(Window));
        public static RoutedCommand ShowWorkspaceExplorerCommand { get; }
            = new RoutedCommand(nameof(ShowWorkspaceExplorerCommand), typeof(Window));
        public static RoutedCommand ShowCodeExplorerCommand { get; }
            = new RoutedCommand(nameof(ShowCodeExplorerCommand), typeof(Window));
        public static RoutedCommand ShowTestExplorerCommand { get; }
            = new RoutedCommand(nameof(ShowTestExplorerCommand), typeof(Window));
        public static RoutedCommand ShowCallHierarchyCommand { get; }
            = new RoutedCommand(nameof(ShowCallHierarchyCommand), typeof(Window));
        public static RoutedCommand ShowObjectBrowserCommand { get; }
            = new RoutedCommand(nameof(ShowObjectBrowserCommand), typeof(Window));
        public static RoutedCommand ShowPropertiesCommand { get; }
            = new RoutedCommand(nameof(ShowPropertiesCommand), typeof(Window));
        public static RoutedCommand ShowDiagnosticsCommand { get; }
            = new RoutedCommand(nameof(ShowDiagnosticsCommand), typeof(Window));
        public static RoutedCommand ShowCodeMetricsCommand { get; }
            = new RoutedCommand(nameof(ShowCodeMetricsCommand), typeof(Window));
        public static RoutedCommand ShowTasksCommand { get; }
            = new RoutedCommand(nameof(ShowTasksCommand), typeof(Window));
        public static RoutedCommand ShowSearchResultsCommand { get; }
            = new RoutedCommand(nameof(ShowSearchResultsCommand), typeof(Window));
        public static RoutedCommand ShowEditorTraceCommand { get; }
            = new RoutedCommand(nameof(ShowEditorTraceCommand), typeof(Window));
        public static RoutedCommand ShowLanguageServerTraceCommand { get; }
            = new RoutedCommand(nameof(ShowLanguageServerTraceCommand), typeof(Window));
        public static RoutedCommand ShowUpdateServerTraceCommand { get; }
            = new RoutedCommand(nameof(ShowUpdateServerTraceCommand), typeof(Window));
    }
}

using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public static class WorkspaceExplorerCommands
    {
        public static RoutedCommand OpenFileCommand { get; }
            = new RoutedCommand(nameof(OpenFileCommand), typeof(WorkspaceExplorerControl));

        public static RoutedCommand RenameUriCommand { get; }
            = new RoutedCommand(nameof(RenameUriCommand), typeof(WorkspaceExplorerControl));

        public static RoutedCommand DeleteUriCommand { get; }
            = new RoutedCommand(nameof(DeleteUriCommand), typeof(WorkspaceExplorerControl));

        public static RoutedCommand CreateFileCommand { get; }
            = new RoutedCommand(nameof(CreateFileCommand), typeof(WorkspaceExplorerControl));

        public static RoutedCommand CreateFolderCommand { get; }
            = new RoutedCommand(nameof(CreateFolderCommand), typeof(WorkspaceExplorerControl));
    }
}

using System.Windows;
using System.Windows.Input;
using Resx = Rubberduck.Resources.v3.RubberduckUICommands;

namespace Rubberduck.UI.Shell.Tools.WorkspaceExplorer
{
    public static class WorkspaceExplorerCommands
    {
        public static RoutedUICommand OpenFileCommand { get; }
            = new RoutedUICommand(Resx.WorkspaceExplorerCommands_OpenFileCommandText, nameof(OpenFileCommand), typeof(WorkspaceExplorerControl));

        public static RoutedUICommand RenameUriCommand { get; }
            = new RoutedUICommand(Resx.WorkspaceExplorerCommands_RenameUriCommandText, nameof(RenameUriCommand), typeof(WorkspaceExplorerControl));

        public static RoutedUICommand DeleteUriCommand { get; }
            = new RoutedUICommand(Resx.WorkspaceExplorerCommands_DeleteUriCommandText, nameof(DeleteUriCommand), typeof(WorkspaceExplorerControl));

        public static RoutedUICommand CreateFileCommand { get; }
            = new RoutedUICommand(Resx.WorkspaceExplorerCommands_CreateFileCommandText, nameof(CreateFileCommand), typeof(WorkspaceExplorerControl));

        public static RoutedUICommand CreateFolderCommand { get; }
            = new RoutedUICommand(Resx.WorkspaceExplorerCommands_CreateFolderCommandText, nameof(CreateFolderCommand), typeof(WorkspaceExplorerControl));
    }
}

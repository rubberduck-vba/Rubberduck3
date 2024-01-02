using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class FileCommands
    {
        public static RoutedCommand NewProjectCommand { get; }
            = new RoutedCommand(nameof(NewProjectCommand), typeof(Window));
        public static RoutedCommand OpenProjectCommand { get; }
            = new RoutedCommand(nameof(OpenProjectCommand), typeof(Window));

        public static RoutedCommand SaveDocumentCommand { get; }
            = new RoutedCommand(nameof(SaveDocumentCommand), typeof(Window));
        public static RoutedCommand SaveDocumentAsCommand { get; }
            = new RoutedCommand(nameof(SaveDocumentAsCommand), typeof(Window));
        public static RoutedCommand SaveAllDocumentsCommand { get; }
            = new RoutedCommand(nameof(SaveAllDocumentsCommand), typeof(Window));
        public static RoutedCommand SaveProjectAsTemplateCommand { get; }
            = new RoutedCommand(nameof(SaveProjectAsTemplateCommand), typeof(Window));

        public static RoutedCommand CloseDocumentCommand { get; }
            = new RoutedCommand(nameof(CloseDocumentCommand), typeof(Window));
        public static RoutedCommand CloseAllDocumentsCommand { get; }
            = new RoutedCommand(nameof(CloseAllDocumentsCommand), typeof(Window));
        public static RoutedCommand CloseWorkspaceCommand { get; }
            = new RoutedCommand(nameof(CloseWorkspaceCommand), typeof(Window));
        public static RoutedCommand RenameWorkspaceCommand { get; }
            = new RoutedCommand(nameof(RenameWorkspaceCommand), typeof(Window));


        public static RoutedCommand SynchronizeWorkspaceCommand { get; }
            = new RoutedCommand(nameof(SynchronizeWorkspaceCommand), typeof(Window));
        public static RoutedCommand ExitCommand { get; }
            = new RoutedCommand(nameof(ExitCommand), typeof(Window));

        
        public static RoutedCommand OpenFolderInWindowsExplorerCommand { get; }
            = new RoutedCommand(nameof(OpenFolderInWindowsExplorerCommand), typeof(Window));
    }
}

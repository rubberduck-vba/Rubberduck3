using System.Windows.Input;
using System.Windows;
using Resx = Rubberduck.Resources.v3.RubberduckUICommands;

namespace Rubberduck.UI.Command
{
    public static class FileCommands
    {
        public static RoutedUICommand NewProjectCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_NewProjectCommandText, nameof(NewProjectCommand), typeof(Window));
        public static RoutedUICommand OpenProjectWorkspaceCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_OpenProjectCommandText, nameof(OpenProjectWorkspaceCommand), typeof(Window));

        public static RoutedUICommand SaveActiveDocumentCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_SaveActiveDocumentCommandText, nameof(SaveActiveDocumentCommand), typeof(Window));
        public static RoutedUICommand SaveActiveDocumentAsCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_SaveActiveDocumentAsCommandText, nameof(SaveActiveDocumentAsCommand), typeof(Window));
        public static RoutedUICommand SaveAllDocumentsCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_SaveAllDocumentsCommandText, nameof(SaveAllDocumentsCommand), typeof(Window));
        public static RoutedUICommand SaveProjectAsTemplateCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_SaveProjectAsTemplateCommandText, nameof(SaveProjectAsTemplateCommand), typeof(Window));

        public static RoutedUICommand CloseActiveDocumentCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_CloseActiveDocumentCommandText, nameof(CloseActiveDocumentCommand), typeof(Window));
        public static RoutedUICommand CloseAllDocumentsCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_CloseAllDocumentsCommandText, nameof(CloseAllDocumentsCommand), typeof(Window));
        public static RoutedUICommand CloseProjectWorkspaceCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_CloseProjectCommandText, nameof(CloseProjectWorkspaceCommand), typeof(Window));
        public static RoutedUICommand RenameProjectWorkspaceCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_RenameProjectCommandText, nameof(RenameProjectWorkspaceCommand), typeof(Window));

        public static RoutedUICommand SynchronizeProjectWorkspaceCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_SynchronizeProjectCommandText, nameof(SynchronizeProjectWorkspaceCommand), typeof(Window));
        public static RoutedUICommand ExitCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_ExitCommandText, nameof(ExitCommand), typeof(Window));

        public static RoutedUICommand OpenFolderInWindowsExplorerCommand { get; }
            = new RoutedUICommand(Resx.FileCommands_OpenFolderInWindowsExplorerCommandText, nameof(OpenFolderInWindowsExplorerCommand), typeof(Window));
    }
}

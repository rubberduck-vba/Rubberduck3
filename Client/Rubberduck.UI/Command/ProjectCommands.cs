using System.Windows.Input;
using System.Windows;

namespace Rubberduck.UI.Command
{
    public static class ProjectCommands
    {
        public static RoutedCommand AddModuleCommand { get; }
            = new RoutedCommand(nameof(AddModuleCommand), typeof(Window));
        public static RoutedCommand AddClassCommand { get; }
            = new RoutedCommand(nameof(AddClassCommand), typeof(Window));
        public static RoutedCommand AddUserFormCommand { get; }
            = new RoutedCommand(nameof(AddUserFormCommand), typeof(Window));
        public static RoutedCommand AddTestModuleCommand { get; }
            = new RoutedCommand(nameof(AddTestModuleCommand), typeof(Window));
        public static RoutedCommand AddExistingFileCommand { get; }
            = new RoutedCommand(nameof(AddExistingFileCommand), typeof(Window));
        public static RoutedCommand AddNewFileCommand { get; }
            = new RoutedCommand(nameof(AddNewFileCommand), typeof(Window));

        public static RoutedCommand AddFolderCommand { get; }
            = new RoutedCommand(nameof(AddFolderCommand), typeof(Window));

        public static RoutedCommand ProjectReferencesCommand { get; }
            = new RoutedCommand(nameof(ProjectReferencesCommand), typeof(Window));
        public static RoutedCommand ProjectPropertiesCommand { get; }
            = new RoutedCommand(nameof(ProjectPropertiesCommand), typeof(Window));
    }
}

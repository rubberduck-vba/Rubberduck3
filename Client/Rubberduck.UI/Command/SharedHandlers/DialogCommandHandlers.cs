using Rubberduck.UI.Command.StaticRouted;
using System.Windows.Input;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class DialogCommandHandlers
    {
        public static void BrowseLocationCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        public static void BrowseLocationCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => DialogCommands.BrowseLocation((IBrowseFolderModel)e.Parameter);

        public static void BrowseFileOpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        public static void BrowseFileOpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => DialogCommands.BrowseFileOpen((IBrowseFileModel)e.Parameter);

        public static void BrowseFileSaveAsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        public static void BrowseFileSaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => DialogCommands.BrowseFileSaveAs((IBrowseFileModel)e.Parameter);
    }
}

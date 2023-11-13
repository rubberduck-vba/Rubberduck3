using Rubberduck.UI.Command;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
{
    public class DialogCommandHandlers
    {
        public void CloseWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        public void CloseWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => DialogCommands.BrowseLocation((TextBox)e.Parameter);
    }
}

using Rubberduck.UI.NewProject;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
{
    public class ApplicationCommandHandlers
    {
        public ICommand NewProjectCommand { get; init; }

        public ApplicationCommandHandlers(NewProjectCommand newProjectCommand)
        {
            NewProjectCommand = newProjectCommand;
        }

        public void NewProjectCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = NewProjectCommand.CanExecute(e.Parameter);

        public void NewProjectCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => NewProjectCommand.Execute(e.Parameter);
    }
}

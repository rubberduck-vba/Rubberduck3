using Rubberduck.UI.NewProject;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
{
    public class FileCommandHandlers
    {
        public FileCommandHandlers(NewProjectCommand newProjectCommand)
        {
            NewProjectCommand = newProjectCommand;
        }

        public ICommand NewProjectCommand { get; init; }
        public ICommand OpenProjectCommand { get; init; }
        public ICommand SaveDocumentCommand { get; init; }
        public ICommand SaveDocumentAsCommand { get; init; }
        public ICommand SaveAllDocumentsCommand { get; init; }
        public ICommand SaveAsProjectTemplateCommand { get; init; }
        public ICommand CloseDocumentCommand { get; init; }
        public ICommand CloseAllDocumentsCommand { get; init; }
        public ICommand CloseWorkspaceCommand { get; init; }
        public ICommand SynchronizeWorkspaceCommand { get; init; }
        public ICommand ExitCommand { get; init; }

        public IEnumerable<CommandBinding> CreateCommandBindings() => new[]
        {
            new CommandBinding(NewProjectCommand, NewProjectCommandBinding_Executed, NewProjectCommandBinding_CanExecute),
        };

        private void NewProjectCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = NewProjectCommand.CanExecute(e.Parameter);
        private void NewProjectCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => NewProjectCommand.Execute(e.Parameter);

        private void OpenProjectCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = OpenProjectCommand.CanExecute(e.Parameter);
        private void OpenProjectCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => OpenProjectCommand.Execute(e.Parameter);

        private void SaveDocumentCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = SaveDocumentCommand.CanExecute(e.Parameter);
        private void OpenProjectCommandSaveDocumentCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SaveDocumentCommand.Execute(e.Parameter);

        private void SaveDocumentAsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = SaveDocumentAsCommand.CanExecute(e.Parameter);
        private void SaveDocumentAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SaveDocumentAsCommand.Execute(e.Parameter);

        private void SaveAllDocumentsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = SaveAllDocumentsCommand.CanExecute(e.Parameter);
        private void SaveAllDocumentsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SaveAllDocumentsCommand.Execute(e.Parameter);

        private void SaveAsProjectTemplateCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = SaveAsProjectTemplateCommand.CanExecute(e.Parameter);
        private void SaveAsProjectTemplateCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SaveAsProjectTemplateCommand.Execute(e.Parameter);

        private void CloseDocumentCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = CloseDocumentCommand.CanExecute(e.Parameter);
        private void CloseDocumentCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => CloseDocumentCommand.Execute(e.Parameter);

        private void CloseAllDocumentsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = CloseAllDocumentsCommand.CanExecute(e.Parameter);
        private void CloseAllDocumentsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => CloseAllDocumentsCommand.Execute(e.Parameter);

        private void CloseWorkspaceCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = CloseWorkspaceCommand.CanExecute(e.Parameter);
        private void CloseWorkspaceCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => CloseWorkspaceCommand.Execute(e.Parameter);

        private void SynchronizeWorkspaceCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = SynchronizeWorkspaceCommand.CanExecute(e.Parameter);
        private void SynchronizeWorkspaceCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SynchronizeWorkspaceCommand.Execute(e.Parameter);

        private void ExitCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = ExitCommand.CanExecute(e.Parameter);
        private void ExitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => ExitCommand.Execute(e.Parameter);
    }
}

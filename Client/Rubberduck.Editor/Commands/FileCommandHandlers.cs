using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.Editor.Commands
{
    public class FileCommandHandlers : CommandHandlers
    {
        public FileCommandHandlers(NewProjectCommand newProjectCommand,
            OpenProjectCommand openProjectCommand,
            SaveDocumentCommand saveDocumentCommand,
            SaveDocumentAsCommand saveDocumentAsCommand,
            SaveAllDocumentsCommand saveAllDocumentsCommand,
            SaveAsProjectTemplateCommand saveAsProjectTemplateCommand,
            CloseDocumentCommand closeDocumentCommand,
            CloseAllDocumentsCommand closeAllDocumentsCommand,
            CloseWorkspaceCommand closeWorkspaceCommand,
            SynchronizeWorkspaceCommand synchronizeWorkspaceCommand,
            ExitCommand exitCommand)
        {
            NewProjectCommand = newProjectCommand;
            OpenProjectCommand = openProjectCommand;
            SaveDocumentCommand = saveDocumentCommand;
            SaveDocumentAsCommand = saveDocumentAsCommand;
            SaveAllDocumentsCommand = saveAllDocumentsCommand;
            SaveAsProjectTemplateCommand = saveAsProjectTemplateCommand;
            CloseDocumentCommand = closeDocumentCommand;
            CloseAllDocumentsCommand = closeAllDocumentsCommand;
            CloseWorkspaceCommand = closeWorkspaceCommand;
            SynchronizeWorkspaceCommand = synchronizeWorkspaceCommand;
            ExitCommand = exitCommand;
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

        public override IEnumerable<CommandBinding> CreateCommandBindings() =>
            Bind(
                (FileCommands.NewProjectCommand, NewProjectCommand),
                (FileCommands.OpenProjectWorkspaceCommand, OpenProjectCommand),
                (FileCommands.SaveActiveDocumentCommand, SaveDocumentCommand),
                (FileCommands.SaveActiveDocumentAsCommand, SaveDocumentAsCommand),
                (FileCommands.SaveAllDocumentsCommand, SaveAllDocumentsCommand),
                (FileCommands.SaveProjectAsTemplateCommand, SaveAsProjectTemplateCommand),
                (FileCommands.CloseActiveDocumentCommand, CloseDocumentCommand),
                (FileCommands.CloseAllDocumentsCommand, CloseAllDocumentsCommand),
                (FileCommands.CloseProjectWorkspaceCommand, CloseWorkspaceCommand),
                (FileCommands.SynchronizeProjectWorkspaceCommand, SynchronizeWorkspaceCommand),
                (FileCommands.ExitCommand, ExitCommand)
            );
    }
}

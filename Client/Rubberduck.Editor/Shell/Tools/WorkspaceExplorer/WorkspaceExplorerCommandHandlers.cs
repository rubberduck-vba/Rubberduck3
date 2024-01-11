using Rubberduck.Editor.Commands;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Shell.Tools.WorkspaceExplorer;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.Tools.WorkspaceExplorer
{
    public class WorkspaceExplorerCommandHandlers : CommandHandlers
    {
        public WorkspaceExplorerCommandHandlers(OpenDocumentCommand openDocumentCommand)
        {
            OpenDocumentCommand = openDocumentCommand;
        }

        public override IEnumerable<CommandBinding> CreateCommandBindings() =>
            Bind(
                (WorkspaceExplorerCommands.OpenFileCommand, OpenDocumentCommand)
            );

        public ICommand OpenDocumentCommand { get; init; }
    }
}

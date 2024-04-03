using Rubberduck.UI.Command;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.StaticRouted;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rubberduck.Editor.Commands
{
    public class ViewCommandHandlers : CommandHandlers
    {
        public ViewCommandHandlers(
            ShowWorkspaceExplorerCommand showWorkspaceExplorerCommand,
            ShowLanguageServerTraceCommand showLanguageServerTraceCommand)
        {
            ShowWorkspaceExplorerCommand = showWorkspaceExplorerCommand;
            ShowLanguageServerTraceCommand = showLanguageServerTraceCommand;
        }

        public ICommand ViewCodeCommand { get; init; }
        public ICommand ViewDesignerCommand { get; init; }
        public ICommand ShowWorkspaceExplorerCommand { get; init; }
        public ICommand ShowCodeExplorerCommand { get; init; }
        public ICommand ShowTestExplorerCommand { get; init; }
        public ICommand ShowCallHierarchyCommand { get; init; }
        public ICommand ShowObjectBrowserCommand { get; init; }
        public ICommand ShowPropertiesCommand { get;init; }
        public ICommand ShowDiagnosticsCommand { get; init; }
        public ICommand ShowCodeMetricsCommand { get; init; }
        public ICommand ShowTasksCommand { get; init; }
        public ICommand ShowSearchResultsCommand { get; init; }
        public ICommand ShowEditorTraceCommand { get; init; }
        public ICommand ShowLanguageServerTraceCommand { get; init; }
        public ICommand ShowUpdateServerTraceCommand { get; init; }

        public override IEnumerable<CommandBinding> CreateCommandBindings() => 
            Bind(
                (ViewCommands.ShowWorkspaceExplorerCommand, ShowWorkspaceExplorerCommand),
                (ViewCommands.ShowLanguageServerTraceCommand, ShowLanguageServerTraceCommand)
            );
    }
}

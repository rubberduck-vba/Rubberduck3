using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.WorkspaceExplorer;
using Rubberduck.UI.WorkspaceExplorer;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command
{
    public class ShowWorkspaceExplorerCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;
        public ShowWorkspaceExplorerCommand(UIServiceHelper service,
            IWorkspaceService workspaceService) 
            : base(service)
        {
            _workspace = workspaceService;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var vm = new WorkspaceExplorerWindowViewModel(_workspace);
            if (parameter is ProjectFile projectFile)
            {
                vm.Load(projectFile);
            }

            var view = new WorkspaceExplorerToolWindow() { DataContext = vm };
            view.Show();

            await Task.CompletedTask;
        }
    }
}

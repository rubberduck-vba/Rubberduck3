using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class CloseWorkspaceCommand : CommandBase
    {
        private readonly IAppWorkspacesService _workspace;

        public CloseWorkspaceCommand(UIServiceHelper service,
            IAppWorkspacesService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _workspace.CloseWorkspace();
        }
    }
}

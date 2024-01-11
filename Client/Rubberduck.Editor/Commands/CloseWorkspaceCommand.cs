using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class CloseWorkspaceCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public CloseWorkspaceCommand(UIServiceHelper service,
            IWorkspaceService workspace)
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

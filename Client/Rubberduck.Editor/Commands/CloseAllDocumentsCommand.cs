using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class CloseAllDocumentsCommand : CommandBase
    {
        private readonly IAppWorkspacesService _workspace;

        public CloseAllDocumentsCommand(UIServiceHelper service,
            IAppWorkspacesService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            _workspace.CloseAllFiles();
        }
    }
}

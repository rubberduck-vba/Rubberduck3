using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class CloseAllDocumentsCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public CloseAllDocumentsCommand(UIServiceHelper service,
            IWorkspaceService workspace)
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

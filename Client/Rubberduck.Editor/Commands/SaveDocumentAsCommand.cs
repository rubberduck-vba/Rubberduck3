using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class SaveDocumentAsCommand : CommandBase
    {
        private readonly IAppWorkspacesService _workspace;

        public SaveDocumentAsCommand(UIServiceHelper service,
            IAppWorkspacesService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            // TODO once there's a document state manager, grab the ActiveDocument here
            // TODO prompt for file name
            //_workspace.SaveWorkspaceFileAsync(uri);
        }
    }
}

using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class SaveDocumentCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public SaveDocumentCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            // TODO once there's a document state manager, grab the ActiveDocument here
            //_workspace.SaveWorkspaceFileAsync(uri);
        }
    }
}

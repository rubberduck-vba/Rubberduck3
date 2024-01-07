using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class CloseDocumentCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public CloseDocumentCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is Uri uri)
            {
                _workspace.CloseFile(uri);
                return;
            }

            // TODO once there's a document state manager, grab the ActiveDocument here
        }
    }
}

using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class SaveAllDocumentsCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public SaveAllDocumentsCommand(UIServiceHelper service,
            IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            await _workspace.SaveAllAsync();
        }
    }
}

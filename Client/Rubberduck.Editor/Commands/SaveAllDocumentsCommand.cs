using Rubberduck.InternalApi.Services;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using System.Threading.Tasks;

namespace Rubberduck.Editor.Commands
{
    public class SaveAllDocumentsCommand : CommandBase
    {
        private readonly IAppWorkspacesService _workspace;

        public SaveAllDocumentsCommand(UIServiceHelper service,
            IAppWorkspacesService workspace)
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

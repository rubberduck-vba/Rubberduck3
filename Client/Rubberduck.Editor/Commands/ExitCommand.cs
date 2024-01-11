using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Threading.Tasks;
using System.Windows;

namespace Rubberduck.Editor.Commands
{
    public class ExitCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;

        public ExitCommand(UIServiceHelper service, IWorkspaceService workspace)
            : base(service)
        {
            _workspace = workspace;
        }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            await _workspace.SaveAllAsync();
            // TODO synchronize workspace
            Application.Current.Shutdown(); // FIXME closes the editor window, but does not exit the process...
        }
    }
}

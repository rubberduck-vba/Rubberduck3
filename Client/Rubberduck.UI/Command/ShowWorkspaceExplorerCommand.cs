using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Tools.WorkspaceExplorer;

namespace Rubberduck.UI.Command
{
    public class ShowWorkspaceExplorerCommand : ShowToolWindowCommand<WorkspaceExplorerControl, IWorkspaceExplorerViewModel>
    {
        public ShowWorkspaceExplorerCommand(UIServiceHelper service, ShellProvider shell, IWorkspaceExplorerViewModel vm)
            : base(service, shell, vm) 
        {
        }
    }
}

using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.WorkspaceExplorer;
using Rubberduck.UI.WorkspaceExplorer;
using System;

namespace Rubberduck.UI.Command
{
    public class ShowWorkspaceExplorerCommand : ShowToolWindowCommand<WorkspaceExplorerControl, WorkspaceExplorerViewModel>
    {
        public ShowWorkspaceExplorerCommand(UIServiceHelper service, ShellProvider shell, WorkspaceExplorerViewModel vm)
            : base(service, shell, vm) 
        {
        }
    }
}

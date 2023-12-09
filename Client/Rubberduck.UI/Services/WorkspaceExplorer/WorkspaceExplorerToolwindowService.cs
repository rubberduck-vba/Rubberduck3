using Rubberduck.UI.Command;
using Rubberduck.UI.Windows;
using Rubberduck.UI.Windows.ToolWindows;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Windows;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceExplorerToolWindowService : IToolWindowService<IWorkspaceExplorerViewModel>
    {
        private readonly ShellProvider _shell;
        private WorkspaceExplorerControl? _control;

        public WorkspaceExplorerToolWindowService(ShellProvider shell)
        {
            _shell = shell;
        }

        private static WorkspaceExplorerControl CreateControl(IWorkspaceExplorerViewModel viewModel)
        {
            return new WorkspaceExplorerControl() { DataContext = viewModel };
        }

        public void Dock(IWorkspaceExplorerViewModel viewModel, DockingLocation location)
        {
            var control = _control ??= CreateControl(viewModel);
            _shell.ViewModel.ToolWindows.Add(viewModel);
        }

        public void Float(IWorkspaceExplorerViewModel viewModel) => Dock(viewModel, DockingLocation.None);
    }
}

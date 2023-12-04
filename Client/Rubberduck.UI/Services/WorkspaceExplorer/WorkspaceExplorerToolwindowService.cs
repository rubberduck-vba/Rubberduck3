using Rubberduck.UI.Windows.ToolWindows;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Windows;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceExplorerToolWindowService : IToolWindowService<IWorkspaceExplorerViewModel>
    {
        private Window? _window;

        private static Window CreateWindow(IWorkspaceExplorerViewModel viewModel)
        {
            return new WorkspaceExplorerToolWindow() { DataContext = viewModel };
        }

        public void Dock(IWorkspaceExplorerViewModel viewModel, ToolDockLocation location)
        {
            var window = _window ??= CreateWindow(viewModel);
            window.Show();
        }

        public void Float(IWorkspaceExplorerViewModel viewModel)
        {
            var window = _window ??= CreateWindow(viewModel);
            window.Show();
        }
    }
}

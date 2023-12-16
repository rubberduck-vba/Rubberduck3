using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.WorkspaceExplorer;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command
{
    public class ShowWorkspaceExplorerCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;
        private readonly ShellProvider _shell;

        private readonly ShowRubberduckSettingsCommand _settingsCommand;
        private readonly CloseToolWindowCommand _closeToolwindowCommand;
        private readonly IWorkspaceExplorerViewModel _vm;

        public ShowWorkspaceExplorerCommand(UIServiceHelper service, ShellProvider shell,
            IWorkspaceService workspaceService, WorkspaceExplorerViewModel explorer,
            ShowRubberduckSettingsCommand showSettingsCommand
,           CloseToolWindowCommand closeToolwindowCommand)
            : base(service)
        {
            _workspace = workspaceService;
            _settingsCommand = showSettingsCommand;
            _closeToolwindowCommand = closeToolwindowCommand;
            _shell = shell;

            _vm = explorer;
        }

        private WorkspaceExplorerControl? _view;
        protected async override Task OnExecuteAsync(object? parameter)
        {
            var shell = _shell.ViewModel;
            _vm.IsSelected = true;

            if (!shell.ToolWindows.Any(e => e.Title == _vm.Title) || _view is null)
            {
                if (_view is null)
                {
                    _view = new WorkspaceExplorerControl { DataContext = _vm };
                    _vm.Content = _view; // <~ FIXME WTF

                    // TODO get from workspace if available, otherwise get from settings/defaults:
                    _shell.View.LeftPaneToolTabs.AddToSource(_view);
                }

                shell.ToolWindows.Add(_vm);
            }

            switch (_vm.DockingLocation)
            {
                case DockingLocation.DockLeft:
                    _shell.View.LeftPaneExpander.IsExpanded = true;
                    break;
                case DockingLocation.DockRight:
                    _shell.View.RightPaneExpander.IsExpanded = true;
                    break;
            }

            await Task.CompletedTask;
        }
    }
}

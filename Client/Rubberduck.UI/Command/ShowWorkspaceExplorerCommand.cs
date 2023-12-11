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
        private readonly Lazy<WorkspaceExplorerViewModel> _vm;

        public ShowWorkspaceExplorerCommand(UIServiceHelper service, ShellProvider shell,
            IWorkspaceService workspaceService,
            ShowRubberduckSettingsCommand showSettingsCommand) 
            : base(service)
        {
            _workspace = workspaceService;
            _settingsCommand = showSettingsCommand;
            _shell = shell;

            _vm = new Lazy<WorkspaceExplorerViewModel>(() => new WorkspaceExplorerViewModel(_workspace, _settingsCommand));
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var shell = _shell.ViewModel;
            var vm = _vm.Value;
            if (!shell.ToolWindows.Any(e => e.Title == vm.Title))
            {
                var view = new WorkspaceExplorerControl();
                view.DataContext = vm;
                vm.Content = view;

                // TODO get from workspace if available, otherwise get from settings/defaults:
                _shell.View.LeftPaneToolTabs.AddToSource(view);
                shell.ToolWindows.Add(vm);
            }

            await Task.CompletedTask;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.WorkspaceExplorer;
using Rubberduck.UI.Shell;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command
{
    public class ShellProvider
    {
        private readonly IServiceProvider _provider;
        private ShellWindow? _shell;

        public ShellProvider(IServiceProvider provider) 
        {
            _provider = provider;
        }

        public IShellWindowViewModel ViewModel => _provider.GetRequiredService<IShellWindowViewModel>();
        public ShellWindow View => _shell ??= _provider.GetRequiredService<Func<ShellWindow>>().Invoke();
    }

    public class ShowWorkspaceExplorerCommand : CommandBase
    {
        private readonly IWorkspaceService _workspace;
        private readonly ShellProvider _shell;

        public ShowWorkspaceExplorerCommand(UIServiceHelper service, ShellProvider shell,
            IWorkspaceService workspaceService) 
            : base(service)
        {
            _workspace = workspaceService;
            _shell = shell;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            var shell = _shell.ViewModel;
            var vm = new WorkspaceExplorerViewModel(_workspace);

            if (!shell.ToolWindows.Any(e => e.Title == vm.Title))
            {
                var view = new WorkspaceExplorerControl();
                view.DataContext = vm;
                vm.Content = view;

                _shell.View.LeftPaneToolTabs.AddToSource(vm);
            }

            await Task.CompletedTask;
        }
    }
}

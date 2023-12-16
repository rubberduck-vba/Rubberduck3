using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class SystemCommandHandlers : CommandHandlers
    {
        private readonly Window _window;

        public SystemCommandHandlers(Window window)
        {
            _window = window;
        }

        public override IEnumerable<CommandBinding> CreateCommandBindings() =>
            Bind(
                (SystemCommands.CloseWindowCommand, param => SystemCommands.CloseWindow(_window)),
                (SystemCommands.ShowSystemMenuCommand, (param) => SystemCommands.ShowSystemMenu(_window, Mouse.GetPosition(_window)))
            ).Concat(
            Bind(
                (SystemCommands.MinimizeWindowCommand, (param) => SystemCommands.MinimizeWindow(_window), param => _window.WindowState != WindowState.Minimized),
                (SystemCommands.MaximizeWindowCommand, (param) => SystemCommands.MaximizeWindow(_window), param => _window.WindowState != WindowState.Maximized),
                (SystemCommands.RestoreWindowCommand, (param) => SystemCommands.RestoreWindow(_window), param => _window.WindowState != WindowState.Normal)
            ));
    }

    public class CloseToolWindowCommand : CommandBase
    {
        private readonly ShellProvider _shell;

        public CloseToolWindowCommand(UIServiceHelper service, ShellProvider shell) 
            : base(service)
        {
            _shell = shell;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            if (parameter is IToolWindowViewModel toolwindow)
            {
                _shell.ViewModel.ToolWindows.Remove(toolwindow);
                if (toolwindow.DockingLocation == DockingLocation.DockLeft)
                {
                    _shell.View.LeftPaneToolTabs.RemoveFromSource(toolwindow);
                }
            }
        }
    }

    public class ToolWindowCommandHandlers : CommandHandlers
    {
        private readonly ICommand _closeCommand;

        public ToolWindowCommandHandlers(CloseToolWindowCommand closeCommand)
        {
            _closeCommand = closeCommand;
        }

        public override IEnumerable<CommandBinding> CreateCommandBindings()
        {
            return Bind((ToolsCommands.CloseToolwindowCommand, _closeCommand));
        }
    }
}

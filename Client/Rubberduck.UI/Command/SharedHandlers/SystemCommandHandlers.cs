using Rubberduck.UI.Command.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
}

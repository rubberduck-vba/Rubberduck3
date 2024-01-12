using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class OpenLogFileCommand : CommandBase
    {
        public OpenLogFileCommand(UIServiceHelper service)
            : base(service)
        {

        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            throw new NotImplementedException();
        }
    }

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
            if (parameter is Window window)
            {
                window.Close();
            }

            /* no good */

            if (parameter is IToolWindowViewModel toolwindow)
            {
                switch (toolwindow.DockingLocation)
                {
                    case DockingLocation.DockLeft:
                        _shell.ViewModel.LeftPanelToolWindows.Remove(toolwindow);
                        _shell.View.LeftPaneToolTabs.RemoveFromSource(toolwindow);
                        _shell.View.LeftPaneExpander.IsExpanded = _shell.ViewModel.LeftPanelToolWindows.Any(e => e.IsPinned && e.DockingLocation == toolwindow.DockingLocation);
                        break;
                    case DockingLocation.DockRight:
                        _shell.ViewModel.RightPanelToolWindows.Remove(toolwindow);
                        _shell.View.RightPaneToolTabs.RemoveFromSource(toolwindow);
                        _shell.View.RightPaneExpander.IsExpanded = _shell.ViewModel.RightPanelToolWindows.Any(e => e.IsPinned && e.DockingLocation == toolwindow.DockingLocation);
                        break;
                    case DockingLocation.DockBottom:
                        _shell.ViewModel.BottomPanelToolWindows.Remove(toolwindow);
                        _shell.View.BottomPaneToolTabs.RemoveFromSource(toolwindow);
                        _shell.View.BottomPaneExpander.IsExpanded = _shell.ViewModel.BottomPanelToolWindows.Any(e => e.IsPinned && e.DockingLocation == toolwindow.DockingLocation);
                        break;
                }
            }
            else if (parameter is IDocumentTabViewModel tab)
            {
                _shell.ViewModel.DocumentWindows.Remove(tab);
                _shell.View.DocumentPaneTabs.RemoveFromSource(tab);
            }

            await Task.CompletedTask;
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

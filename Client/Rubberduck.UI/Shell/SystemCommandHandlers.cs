using Rubberduck.UI.Command;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
{
    public class DialogCommandHandlers
    {
        private readonly Window _window;

        public DialogCommandHandlers(Window window)
        {
            _window = window;
        }

        public void CloseWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        public void CloseWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => DialogCommands.BrowseLocation(_window);
    }

    public class SystemCommandHandlers
    {
        private readonly Window _window;

        public SystemCommandHandlers(Window window) 
        {
            _window = window;
        }

        public void CloseWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
            => e.CanExecute = true;

        public void CloseWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) 
            => SystemCommands.CloseWindow(_window);

        public void MinimizeWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
            => e.CanExecute = _window.WindowState != WindowState.Minimized;

        public void MinimizeWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) 
            => SystemCommands.MinimizeWindow(_window);

        public void MaximizeWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = _window.WindowState != WindowState.Maximized;

        public void MaximizeWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SystemCommands.MaximizeWindow(_window);

        public void RestoreWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
            => e.CanExecute = _window.WindowState == WindowState.Maximized;

        public void RestoreWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
            => SystemCommands.RestoreWindow(_window);

        public void ShowSystemMenuCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) 
            => e.CanExecute = true;

        public void ShowSystemMenuCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var location = Mouse.GetPosition(_window);
            SystemCommands.ShowSystemMenu(_window, location);
        }
    }
}

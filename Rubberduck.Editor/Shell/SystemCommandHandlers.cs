using System.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell
{
    public class SystemCommandHandlers
    {
        private readonly Window _window;

        public SystemCommandHandlers(Window window) 
        {
            _window = window;
        }

        public void CloseWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        public void CloseWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(_window);
            e.Handled = true;
        }

        public void MinimizeWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState != WindowState.Minimized;
            e.Handled = true;
        }

        public void MinimizeWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(_window);
            e.Handled = true;
        }

        public void MaximizeWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState != WindowState.Maximized;
            e.Handled = true;
        }

        public void MaximizeWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(_window);
            e.Handled = true;
        }

        public void RestoreWindowCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState == WindowState.Maximized;
            e.Handled = true;
        }

        public void RestoreWindowCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(_window);
            e.Handled = true;
        }

        public void ShowSystemMenuCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        public void ShowSystemMenuCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var location = Mouse.GetPosition(_window);
            SystemCommands.ShowSystemMenu(_window, location);
            e.Handled = true;
        }
    }
}

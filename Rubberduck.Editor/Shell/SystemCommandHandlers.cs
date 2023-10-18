using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public class SystemCommandHandlers
    {
        private readonly Window _window;

        public SystemCommandHandlers(Window window) 
        {
            _window = window;
        }

        public void CloseWindowCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        public void CloseWindowCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _window.Close();
            e.Handled = true;
        }

        public void MinimizeWindowCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState != WindowState.Minimized;
            e.Handled = true;
        }

        public void MinimizeWindowCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _window.WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        public void MaximizeWindowCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState != WindowState.Maximized;
            e.Handled = true;
        }

        public void MaximizeWindowCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _window.WindowState = WindowState.Maximized;

            e.Handled = true;
        }

        public void RestoreWindowCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _window.WindowState == WindowState.Maximized;
            e.Handled = true;
        }

        public void RestoreWindowCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _window.WindowState = WindowState.Normal;
            e.Handled = true;
        }
    }
}

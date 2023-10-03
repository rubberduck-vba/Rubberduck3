using Rubberduck.UI;
using Rubberduck.UI.Xaml;
using System.Windows;

namespace Rubberduck.Core.Editor
{
    internal class EditorShellWindowPresenter : IPresenter
    {
        public EditorShellWindowPresenter(ShellWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            View = new ShellWindow(viewModel);
        }

        public Window View { get; init; }
        public ShellWindowViewModel ViewModel { get; init; }

        public void Hide()
        {
            View.Hide();
        }

        public void Show()
        {
            View.Show();
            View.Focus();
            View.Closing += View_Closing;
        }

        public void Close()
        {
            View.Closing -= View_Closing;
            View.Close();
        }

        private void View_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            View.Hide();
        }
    }
}
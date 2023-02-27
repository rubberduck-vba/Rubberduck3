using Rubberduck.Client.LocalDb.UI.Xaml;

namespace Rubberduck.Client.LocalDb.UI
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindowFactory(MainWindowViewModel viewModel)
        { 
            _viewModel = viewModel;
        }

        public MainWindow Create()
        {
            return new MainWindow
            {
                DataContext = _viewModel
            };
        }
    }
}

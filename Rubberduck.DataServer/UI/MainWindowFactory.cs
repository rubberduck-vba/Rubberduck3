using Rubberduck.DataServer.UI.Xaml;
using Rubberduck.RPC.Platform;

namespace Rubberduck.DataServer.UI
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindowFactory(MainWindowViewModel viewModel)
        { 
            _viewModel = viewModel;
        }

        public MainWindow Create(IJsonRpcServer server)
        {
            return new MainWindow
            {
                DataContext = _viewModel
            };
        }
    }
}

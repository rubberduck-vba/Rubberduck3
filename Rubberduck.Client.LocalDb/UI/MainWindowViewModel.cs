using Rubberduck.UI;
using Rubberduck.UI.Abstract;

namespace Rubberduck.Client.LocalDb
{
    public class MainWindowViewModel : ViewModelBase, IDataServerMainWindowViewModel
    {
        public MainWindowViewModel(IConsoleViewModel console, IServerStatusViewModel status)
        {
            Console = console;
            Status = status;
        }

        public IConsoleViewModel Console { get; }
        public IServerStatusViewModel Status { get; }
    }
}

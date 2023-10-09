using Rubberduck.UI.Xaml.Dependencies.Controls.ServerStatus;
using Rubberduck.UI.Xaml.ServerTrace;

namespace Rubberduck.UI.Abstract.DataServer
{
    public interface IDataServerMainWindowViewModel
    {
        IConsoleViewModel Console { get; }
        IServerStatusViewModel Status { get; }
    }
}

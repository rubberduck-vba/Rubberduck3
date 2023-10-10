using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI.Xaml.ServerTrace;

namespace Rubberduck.UI.Abstract.DataServer
{
    public interface IDataServerMainWindowViewModel
    {
        IConsoleViewModel Console { get; }
        IServerStatusViewModel Status { get; }
    }
}

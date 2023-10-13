using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI.Xaml.ServerTrace;

namespace Rubberduck.UI.RubberduckEditor.Proto.DataServer
{
    public interface IDataServerMainWindowViewModel
    {
        IConsoleViewModel Console { get; }
        IServerStatusViewModel Status { get; }
    }
}

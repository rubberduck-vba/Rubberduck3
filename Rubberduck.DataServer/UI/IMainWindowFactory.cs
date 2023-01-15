using Rubberduck.DataServer.UI.Xaml;
using Rubberduck.RPC.Platform;

namespace Rubberduck.DataServer
{
    internal interface IMainWindowFactory
    {
        MainWindow Create(IJsonRpcServer server);
    }
}

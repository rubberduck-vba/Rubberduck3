using Rubberduck.Client.LocalDb.UI.Xaml;
using Rubberduck.RPC.Platform;

namespace Rubberduck.Client.LocalDb
{
    internal interface IMainWindowFactory
    {
        MainWindow Create(IJsonRpcServer server);
    }
}

using Rubberduck.RPC.Platform;
using System.Threading;
using System.Windows;

namespace Rubberduck.Client.LocalDb
{
    internal class App : Application 
    {
        private readonly IMainWindowFactory _factory;
        private IJsonRpcServer _server;

        public App(IJsonRpcServer server, IMainWindowFactory factory)
        {
            _server = server;
            _factory = factory;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _server.RunAsync(CancellationToken.None);
        }
    }
}

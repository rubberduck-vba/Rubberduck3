using Rubberduck.DataServer.Properties;
using Rubberduck.RPC.Platform;
using System;
using System.Linq;
using System.Windows;

namespace Rubberduck.DataServer
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

        public void InitializeComponent()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _server.Start();

            if (_server.IsInteractive)
            {
                var view = _factory.Create(_server);
                view.Show();
            }
        }
    }
}

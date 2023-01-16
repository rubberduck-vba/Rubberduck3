using Rubberduck.Client.LocalDb.Properties;
using Rubberduck.RPC.Platform;
using System;
using System.Linq;
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
        }

        public void InitializeComponent()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _server.Start();
        }
    }
}

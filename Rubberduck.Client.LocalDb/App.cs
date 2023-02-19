using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Rubberduck.Client.LocalDb
{
    internal class App : Application 
    {
        private readonly IMainWindowFactory _factory;
        private readonly ILanguageClient _server;
        
        public App(IMainWindowFactory factory, ILanguageClient server)
        {
            _factory = factory;
            _server = server;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _ = StartupAsync();
        }

        private async Task StartupAsync()
        {

            //var response = await _server.Initialize(token);

            //var serverInfo = response.ServerInfo;
            //var serverConfig = response.Capabilities;

            //var statusService = new ServerStatusViewModel(_server);
            //var consoleService = new ConsoleViewModel(_server, _console);

            var window = _factory.Create();
            window.Show();
        }
    }
}

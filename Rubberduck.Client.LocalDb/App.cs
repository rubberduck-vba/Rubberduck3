using Rubberduck.RPC.Platform;
using System.Threading;
using System.Windows;

namespace Rubberduck.Client.LocalDb
{
    internal class App : Application 
    {
        private readonly IMainWindowFactory _factory;

        public App(IMainWindowFactory factory)
        {
            _factory = factory;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
        }
    }
}

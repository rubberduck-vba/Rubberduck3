using Rubberduck.Client.LocalDb.UI;
using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using System;
using System.Diagnostics;

namespace Rubberduck.Client.LocalDb
{
    public static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var options = StartupOptions.Validate(args);
            var process = GetOrCreateServer(options);

            var rpcStreamFactory = new NamedPipeClientStreamFactory("Rubberduck.Server.LocalDb.RPC");
            var serverProxy = new LocalDbServerProxyClient(rpcStreamFactory);
            var consoleProxy = new LocalDbServerConsoleProxyClient(rpcStreamFactory);

            var statusVM = new ServerStatusViewModel(serverProxy);
            var consoleVM = new ConsoleViewModel(serverProxy, null as IServerConsoleProxyClient);

            var vm = new MainWindowViewModel(consoleVM, statusVM);
            var factory = new MainWindowFactory(vm);

            var app = new App(factory, serverProxy, consoleProxy);
            app.Run();

            process.OutputDataReceived -= OnServerProcessStdOut;
            process.ErrorDataReceived -= OnServerProcessStdErr;
            process.Exited -= OnServerProcessExit;
        }

        private static Process GetOrCreateServer(StartupOptions startupOptions)
        {
            var process = ServerProcessClientHelper.StartLocalDb(hidden: false);
            
            process.OutputDataReceived += OnServerProcessStdOut;
            process.ErrorDataReceived += OnServerProcessStdErr;
            process.Exited += OnServerProcessExit;
            
            return process;
        }

        private static void OnServerProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("Rubberduck.Server.LocalDb process has exited unexpectedly.");
        }

        private static void OnServerProcessStdErr(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private static void OnServerProcessStdOut(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }
    }
}

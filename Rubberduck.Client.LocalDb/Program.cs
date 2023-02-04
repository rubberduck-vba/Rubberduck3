using Rubberduck.Client.LocalDb.UI;
using Rubberduck.Client.LocalDb.UI.Commands;
using Rubberduck.RPC;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
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
            var serverProcess = GetOrCreateServer(options);

            //IMainWindowFactory factory = null;
            //var shutdownCommand = new ShutdownCommand(server);

            //var copyCommand = new CopyCommand(console);
            //var saveAsCommand = new SaveAsCommand(server, new FileNameProvider());
            //var pauseTraceCommand = new PauseTraceCommand(console);
            //var resumeTraceCommand = new ResumeTraceCommand(console);
            //var setTraceCommand = new SetTraceCommand(console);

            //var statusVM = new ServerStatusViewModel(server, server as ILocalDbServerEvents);
            //var consoleVM = new ConsoleViewModel(server, console,
            //    shutdownCommand, copyCommand, saveAsCommand, pauseTraceCommand, resumeTraceCommand, setTraceCommand);

            //var vm = new MainWindowViewModel(consoleVM, statusVM);
            //factory = new MainWindowFactory(vm);

            //App app = new App(server, factory);
            //app.Run();
        }

        private static Process GetOrCreateServer(StartupOptions startupOptions)
        {
            var process = ServerProcessClientHelper.StartLocalDb(hidden: true);
            
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

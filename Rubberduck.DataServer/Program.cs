using CommandLine;
using Rubberduck.Client.LocalDb.Properties;
using Rubberduck.Client.LocalDb.UI;
using Rubberduck.Client.LocalDb.UI.Commands;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Rubberduck.Client.LocalDb
{
    public static class Program
    {
        public class Options
        {
            [Option('p', "port", Required = true, HelpText = "Sets the RPC port for this server (required).")]
            public int Port { get; set; }

            [Option('i', "interactive", Required = false, HelpText = "Whether this server should display a UI.")]
            public bool Interactive { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Whether this server logs verbose messages.")]
            public bool Verbose { get; set; }

            [Option('s', "silent", Required = false, HelpText = "Whether trace output is disabled on this server.")]
            public bool Silent { get; set; }

            [Option('d', "exit-delay", Required = false, Default = 2000, HelpText = "The number of milliseconds to wait before exiting the process after shutting down.")]
            public int ExitDelayMilliseconds { get; set; }
        }

        private static TimeSpan _exclusiveAccessTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(errors =>
                {
                    using (var stdErr = Console.OpenStandardError())
                    {
                        var message = Encoding.UTF8.GetBytes("Errors have occurred processing command-line arguments. Server will not be started.");
                        stdErr.Write(message, 0, message.Length);
                    }
                })
                .WithParsed(options =>
                {
                    using (var stdOut = Console.OpenStandardOutput()) 
                    {
                        var message = Encoding.UTF8.GetBytes("Command-line arguments successfully parsed. Proceeding with startup...");
                        stdOut.Write(message, 0, message.Length);
                    }
                });

            if (result.Errors.Any())
            {
                throw new ArgumentException("Invalid command-line arguments were supplied.");
            }
            var startupOptions = result.Value;
            ValidatePort(startupOptions.Port);

            LocalDbServerProcess.Start(startupOptions.Port, startupOptions.Interactive);

            IMainWindowFactory factory = null;
            if (startupOptions.Interactive)
            {/*
                var environment = new EnvironmentService();
                var shutdownCommand = new ShutdownCommand(server, environment);
                var copyCommand = new CopyCommand(console);
                var saveAsCommand = new SaveAsCommand(server, new FileNameProvider());
                var pauseTraceCommand = new PauseTraceCommand(console);
                var resumeTraceCommand = new ResumeTraceCommand(console);
                var setTraceCommand = new SetTraceCommand(console);

                var statusVM = new ServerStatusViewModel(server, server as ILocalDbServerEvents);
                var consoleVM = new ConsoleViewModel(server, console,
                    shutdownCommand, copyCommand, saveAsCommand, pauseTraceCommand, resumeTraceCommand, setTraceCommand);

                var vm = new MainWindowViewModel(consoleVM, statusVM);
                factory = new MainWindowFactory(vm);
                */
            }

            /*
            App app = new App(server, factory);
            app.InitializeComponent();
            app.Run();
            */
        }

        private static void ValidatePort(int port)
        {
            if (port < 1024 || port > 5000)
            {
                throw new ArgumentOutOfRangeException("Invalid RPC port. Must be in the range 1024-5000.");
            }
        }
    }
}

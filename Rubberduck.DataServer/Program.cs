using CommandLine;
using Rubberduck.DataServer.Abstract;
using Rubberduck.DataServer.Properties;
using Rubberduck.DataServer.Services;
using Rubberduck.DataServer.UI;
using Rubberduck.DataServer.UI.Commands;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.RPC.Platform;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Rubberduck.DataServer
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

            [Option('s', "silent", Required = false, HelpText = "Whether this trace is disabled on this server.")]
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
            #region Global Mutex https://stackoverflow.com/a/229567/1188513

            // get application GUID as defined in AssemblyInfo.cs
            var appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(GuidAttribute), false)
                .GetValue(0)).Value
                .ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            var mutexId = $"Global\\{{{appGuid}}}";

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule = new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            using (var mutex = new Mutex(false, mutexId, out _, securitySettings))
            {
                // edited by acidzombie24
                var hasHandle = false;
                try
                {
                    try
                    {
                        // edited by acidzombie24
                        hasHandle = mutex.WaitOne(_exclusiveAccessTimeout.Milliseconds, false);
                        if (!hasHandle) throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact that the mutex was abandoned in another process,
                        // it will still get acquired
                        hasHandle = true;
                    }

                    // Perform your work here.
                    Start(args);
                }
                finally
                {
                    // edited by acidzombie24, added if statement
                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
            #endregion
        }

        private static void Start(string[] args)
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

            var console = new JsonRpcConsole
            {
                IsEnabled = !startupOptions.Silent,
                Trace = startupOptions.Silent ? Constants.TraceValue.Off
                    : startupOptions.Verbose
                        ? Constants.TraceValue.Verbose
                        : Constants.TraceValue.Messages
            };
            var server = new Server(Settings.Default.JsonRpcServerPath, startupOptions.Port, console, startupOptions.Interactive, TimeSpan.FromMilliseconds(startupOptions.ExitDelayMilliseconds));

            IMainWindowFactory factory = null;
            if (startupOptions.Interactive)
            {
                var environment = new EnvironmentService();
                var shutdownCommand = new ShutdownCommand(server, environment);
                var copyCommand = new CopyCommand(console);
                var saveAsCommand = new SaveAsCommand(server, new FileNameProvider());
                var pauseTraceCommand = new PauseTraceCommand(console);
                var resumeTraceCommand = new ResumeTraceCommand(console);
                var setTraceCommand = new SetTraceCommand(console);

                var statusVM = new ServerStatusViewModel(server, server as IDataServerEvents);
                var consoleVM = new ConsoleViewModel(server, console,
                    shutdownCommand, copyCommand, saveAsCommand, pauseTraceCommand, resumeTraceCommand, setTraceCommand);

                var vm = new MainWindowViewModel(consoleVM, statusVM);
                factory = new MainWindowFactory(vm);
            }

            App app = new App(server, factory);
            app.InitializeComponent();
            app.Run();
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

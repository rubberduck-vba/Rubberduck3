using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
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
        private readonly LocalDbServerProxyClient _server;
        private readonly IServerConsoleProxyClient _console;
        
        public App(IMainWindowFactory factory, LocalDbServerProxyClient server, IServerConsoleProxyClient console)
        {
            _factory = factory;
            _server = server;
            _console = console;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _ = StartupAsync();
        }

        private async Task StartupAsync()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var process = Process.GetCurrentProcess();

            var clientInfo = new ClientInfo
            {
                Name = assembly.Name,
                Version = assembly.Version.ToString(3),
                ProcessId = process.Id
            };

            var parameter = new InitializeParams<LocalDbServerCapabilities>
            {
                ClientInfo = clientInfo,
                InitializationOptions = new LocalDbServerCapabilities
                {
                    ConsoleOptions = new ServerConsoleOptions
                    {
                        LogLevel = ServerLogLevel.Trace,
                        IsEnabled = true,
                        Trace = Constants.Console.VerbosityOptions.AsStringEnum.Verbose, // TODO 
                        ConsoleOutputFormatting = new ConsoleOutputFormatOptions
                        {
                            FontFormatting = new FontFormattingOptions
                            {
                                LogLevelFont = new FontOptions
                                {
                                    FontFamily = "Consolas",
                                    FontSize = 10,
                                    FontWeight = Constants.Console.FontWeightOptions.AsFlagsEnum.SemiBold,
                                    ForegroundColorProvider = new ConsoleColorOptions
                                    {
                                        Info = System.ConsoleColor.Green,
                                        Warn = System.ConsoleColor.Yellow,
                                        Error = System.ConsoleColor.White,
                                        Fatal = System.ConsoleColor.White,
                                    },
                                },
                            },
                            BackgroundFormatting = new BackgroundFormattingOptions
                            {
                                LogLevelBackgroundProvider = new ConsoleColorOptions
                                {
                                    Error = System.ConsoleColor.DarkRed,
                                    Fatal = System.ConsoleColor.DarkRed,
                                },
                            }
                        }
                    },
                    // TODO rest of dbserver config?
                },
                Locale = Thread.CurrentThread.CurrentUICulture.Name,
                ProcessId = process.Id,
                Trace = Constants.TraceValue.AsStringEnum.Verbose,
                WorkDoneToken = null, // TODO WorkDoneProgressService
            };

            var token = CancellationToken.None;

            var response = await _server.RequestAsync(proxy => proxy.InitializeClientAsync(parameter));

            var serverInfo = response.ServerInfo;
            var serverConfig = response.Capabilities;

            var statusService = new ServerStatusViewModel(_server);
            var consoleService = new ConsoleViewModel(_server, _console);

            var window = _factory.Create();
            window.Show();
        }
    }
}

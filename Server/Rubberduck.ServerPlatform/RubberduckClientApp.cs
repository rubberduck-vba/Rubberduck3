using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rubberduck.ServerPlatform
{
    public abstract class RubberduckClientApp : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;
        private readonly IServiceProvider _services;

        private Process? _serverProcess = default;
        private NamedPipeClientStream? _clientPipe = default;
        private LanguageClient? _client = default;

        protected RubberduckClientApp(ILogger logger,
            ServerStartupOptions options,
            CancellationTokenSource tokenSource,
            IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _tokenSource = tokenSource;
            _services = services;
        }

        public LanguageClient? LanguageClient => _client;

        public async Task StartupAsync(Uri? workspaceRoot = null)
        {
            if (workspaceRoot != null)
            {
                _options.WorkspaceRoot = workspaceRoot.LocalPath;
            }

            StartServerProcess();

            _logger.LogInformation("Creating client...");
            _client = await LanguageClient.From(ConfigureClient, _services, _tokenSource.Token);
        }

        private void StartServerProcess()
        {
            var settings = _services
                .GetRequiredService<RubberduckSettingsProvider>().Settings
                .LanguageServerSettings.StartupSettings;
            var clientProcessId = Environment.ProcessId;

            _serverProcess = GetServerProcess().Start(clientProcessId, settings, HandleServerExit);
            _logger.LogInformation("Server process has started.");
        }

        public async Task ExitAsync()
        {
            _client?.SendShutdown(new());
            await SendExitServerNotificationAsync();
        }

        private async Task SendExitServerNotificationAsync()
        {
            var delay = _services.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.ExitNotificationDelay;

            await Task.Delay(delay)
                .ContinueWith(o => _client?.SendExit(new()), _tokenSource.Token, TaskContinuationOptions.None, TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private void HandleServerExit(object? sender, EventArgs e)
        {
            _logger.LogInformation("Server process has exited.");
        }

        protected abstract ServerProcess GetServerProcess();

        private void ConfigureClient(LanguageClientOptions options)
        {
            var settings = _services.GetRequiredService<RubberduckSettingsProvider>();

            var resolver = new WorkspaceRootResolver(_logger, settings);
            var workspaceRoot = resolver.GetWorkspaceRootUri(_options);

            ConfigureClient(options, settings.Settings, workspaceRoot);
        }

        private void ConfigureClient(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            var type = _options.TransportType;
            if (_options.ClientProcessId == default)
            {
                type = settings.LanguageServerSettings.StartupSettings.ServerTransportType;
            }

            switch (type)
            {
                case TransportType.StdIO:
                    ConfigureStdIO(options, settings, workspaceRoot);
                    break;

                case TransportType.Pipe:
                    ConfigurePipeIO(options, settings, workspaceRoot);
                    break;

                default:
                    _logger?.LogWarning("An unsupported transport type was specified.");
                    throw new UnsupportedTransportTypeException(_options.TransportType);
            }

            _logger.LogInformation($"Client configuration completed.");
        }

        private void ConfigureStdIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            _logger.LogInformation($"Configuring client transport to use standard input/output streams...");
            var serverProcess = _serverProcess ?? throw new InvalidOperationException("BUG: Server process is not initialized.");

            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            _logger.LogInformation("Configuring client options...");

            var service = _services.GetRequiredService<ILanguageClientService>();
            service.ConfigureLanguageClient(options, Assembly.GetExecutingAssembly(), Environment.ProcessId, settings, workspaceRoot.LocalPath);
        }

        private void ConfigurePipeIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            var pipeName = settings.LanguageServerSettings.StartupSettings.ServerPipeName;
            if (_options is PipeServerStartupOptions ioOptions)
            {
                pipeName = PipeServerStartupOptions.GetPipeName(ioOptions.PipeName, Environment.ProcessId);
            }
            else
            {
                pipeName = PipeServerStartupOptions.GetPipeName(pipeName, Environment.ProcessId);
            }
            _logger.LogInformation("Configuring client transport to use a named pipe stream (name: {pipeName})...", pipeName);

            _clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, System.IO.Pipes.PipeOptions.CurrentUserOnly | System.IO.Pipes.PipeOptions.Asynchronous);
            options.WithInput(PipeReader.Create(_clientPipe));
            options.WithOutput(PipeWriter.Create(_clientPipe));

            var elapsedForConnect = TimedAction.Run(() =>
            {
                _clientPipe.Connect(TimeSpan.FromSeconds(5));
            });
            _logger.LogTrace("Pipe client connected. Elapsed: {elapsedForConnect} (timeout 5 seconds)", elapsedForConnect);

            _logger.LogInformation("Configuring client options...");
            var service = _services.GetRequiredService<ILanguageClientService>();
            service.ConfigureLanguageClient(options, Assembly.GetExecutingAssembly(), Environment.ProcessId, settings, workspaceRoot.LocalPath);
        }

        public void Dispose()
        {
            _serverProcess?.Dispose();
            _clientPipe?.Dispose();
            _client?.Dispose();

            _serverProcess = null;
            _clientPipe = null;
            _client = null;
        }
    }
}
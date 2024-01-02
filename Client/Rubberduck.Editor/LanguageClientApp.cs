using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using Rubberduck.Editor.RPC;
using Rubberduck.Editor.RPC.LanguageServerClient;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using OmniSharpLanguageClient = OmniSharp.Extensions.LanguageServer.Client.LanguageClient;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Configures LSP for editor <--> language server communications.
    /// </summary>
    public sealed class LanguageClientApp : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private Process? _languageServerProcess = default;
        private NamedPipeClientStream? _languageClientPipe = default;
        private OmniSharpLanguageClient? _languageClient = default;

        public LanguageClientApp(ILogger<LanguageClientApp> logger, ServerStartupOptions options, CancellationTokenSource tokenSource, IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _services = services;
            _tokenSource = tokenSource;
        }

        public OmniSharpLanguageClient? LanguageClient => _languageClient;

        public async Task StartupAsync(Uri? workspaceRoot = null)
        {
            if (workspaceRoot != null)
            {
                _options.WorkspaceRoot = workspaceRoot.LocalPath;
            }

            StartLanguageServerProcess();

            _logger.LogInformation("Creating language client...");
            _languageClient = await OmniSharpLanguageClient.From(ConfigureClient, _services, _tokenSource.Token);
        }

        public async Task ExitAsync()
        {
            _languageClient?.SendShutdown(new());
            await SendExitServerNotificationAsync();
        }

        private async Task SendExitServerNotificationAsync()
        {
            var delay = _services.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.ExitNotificationDelay;

            await Task.Delay(delay)
                .ContinueWith(o => _languageClient?.SendExit(new()), _tokenSource.Token, TaskContinuationOptions.None, TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private void StartLanguageServerProcess()
        {
            var settings = _services
                .GetRequiredService<RubberduckSettingsProvider>().Settings
                .LanguageServerSettings.StartupSettings;
            var clientProcessId = Environment.ProcessId;

            _languageServerProcess = new LanguageServerProcess(_logger)
                .Start(clientProcessId, settings, HandleServerExit);
        }

        private void HandleServerExit(object? sender, EventArgs e)
        {
            _logger.LogInformation("LanguageServer process has exited.");
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            var provider = _services;
            var settings = provider.GetRequiredService<RubberduckSettingsProvider>();

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

            _logger.LogInformation($"Language client configuration completed.");
        }

        private void ConfigureStdIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            _logger.LogInformation($"Configuring language client transport to use standard input/output streams...");
            var serverProcess = _languageServerProcess ?? throw new InvalidOperationException("BUG: Server process is not initialized.");

            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            _logger.LogInformation("Configuring language client options...");
            
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
            _logger.LogInformation("Configuring language client transport to use a named pipe stream (name: {pipeName})...", pipeName);

            _languageClientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, System.IO.Pipes.PipeOptions.CurrentUserOnly | System.IO.Pipes.PipeOptions.Asynchronous);
            options.WithInput(PipeReader.Create(_languageClientPipe));
            options.WithOutput(PipeWriter.Create(_languageClientPipe));

            var elapsedForConnect = TimedAction.Run(() =>
            {
                _languageClientPipe.Connect(TimeSpan.FromSeconds(5));
            });
            _logger.LogTrace("Pipe client connected. Elapsed: {elapsedForConnect} (timeout 5 seconds)", elapsedForConnect);

            _logger.LogInformation("Configuring language client options...");
            var service = _services.GetRequiredService<ILanguageClientService>();
            service.ConfigureLanguageClient(options, Assembly.GetExecutingAssembly(), Environment.ProcessId, settings, workspaceRoot.LocalPath);
        }

        public void Dispose()
        {
            _languageServerProcess?.Dispose();
            _languageClientPipe?.Dispose();
            _languageClient?.Dispose();

            _languageServerProcess = null;
            _languageClientPipe = null;
            _languageClient = null;
        }
    }
}

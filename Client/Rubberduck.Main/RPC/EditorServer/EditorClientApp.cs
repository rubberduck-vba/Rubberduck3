using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using Rubberduck.InternalApi.Common;
using Rubberduck.ServerPlatform;
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
using Env = System.Environment;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using OmniSharpLanguageClient = OmniSharp.Extensions.LanguageServer.Client.LanguageClient;

namespace Rubberduck.Main.RPC.EditorServer
{

    public sealed class EditorClientApp : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private Process? _editorServerProcess = default;
        private NamedPipeClientStream? _editorClientPipe = default;
        private OmniSharpLanguageClient? _editorClient = default;

        public EditorClientApp(ILogger<EditorClientApp> logger, ServerStartupOptions options, CancellationTokenSource tokenSource, IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _services = services;
            _tokenSource = tokenSource;
        }

        public OmniSharpLanguageClient? EditorClient => _editorClient;

        public async Task StartupAsync(Uri? workspaceRoot = null)
        {
            StartEditorServerProcess();
            _editorClient = await OmniSharpLanguageClient.From(ConfigureClient, _services, _tokenSource.Token);
        }

        public async Task ExitAsync()
        {
            _editorClient?.SendShutdown(new());
            await SendExitServerNotificationAsync();
        }

        private async Task SendExitServerNotificationAsync()
        {
            var delay = _services.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.ExitNotificationDelay;

            await Task.Delay(delay)
                .ContinueWith(o => _editorClient?.SendExit(new()), _tokenSource.Token, TaskContinuationOptions.None, TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private void StartEditorServerProcess()
        {
            var settings = _services
                .GetRequiredService<RubberduckSettingsProvider>().Settings
                .LanguageServerSettings.StartupSettings;
            var clientProcessId = Env.ProcessId;

            _editorServerProcess = new EditorServerProcess(_logger)
                .Start(clientProcessId, settings, HandleServerExit);
        }

        private void HandleServerExit(object? sender, EventArgs e)
        {
            _logger.LogInformation("EditorServer process has exited.");
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

            _logger.LogInformation($"Editor client configuration completed.");
        }

        private void ConfigureStdIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            _logger.LogInformation($"Configuring editor client transport to use standard input/output streams...");
            var serverProcess = _editorServerProcess ?? throw new InvalidOperationException("BUG: Server process is not initialized.");

            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            var service = _services.GetRequiredService<ILanguageClientService>();
            service.ConfigureLanguageClient(options, Assembly.GetExecutingAssembly(), Env.ProcessId, settings, workspaceRoot.LocalPath);
        }

        private void ConfigurePipeIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            var processId = Env.ProcessId;
            var pipeName = settings.LanguageClientSettings.StartupSettings.ServerPipeName;
            if (_options is PipeServerStartupOptions ioOptions)
            {
                pipeName = PipeServerStartupOptions.GetPipeName(ioOptions.PipeName, Env.ProcessId);
            }
            else
            {
                pipeName = PipeServerStartupOptions.GetPipeName(pipeName, Env.ProcessId);
            }
            _logger.LogInformation("Configuring editor client transport to use a named pipe stream (name: {pipeName})...", pipeName);

            _editorClientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, System.IO.Pipes.PipeOptions.CurrentUserOnly | System.IO.Pipes.PipeOptions.Asynchronous);
            options.WithInput(PipeReader.Create(_editorClientPipe));
            options.WithOutput(PipeWriter.Create(_editorClientPipe));

            var elapsedForConnect = TimedAction.Run(() =>
            {
                _editorClientPipe.Connect(TimeSpan.FromSeconds(5));
            });
            _logger.LogTrace("Pipe client connected. Elapsed: {elapsedForConnect} (timeout 5 seconds)", elapsedForConnect);

            var service = _services.GetRequiredService<ILanguageClientService>();
            service.ConfigureLanguageClient(options, Assembly.GetExecutingAssembly(), Env.ProcessId, settings, workspaceRoot.LocalPath);
        }

        public void Dispose()
        {
            _editorServerProcess?.Dispose();
            _editorClientPipe?.Dispose();
            _editorClient?.Dispose();

            _editorServerProcess = null;
            _editorClientPipe = null;
            _editorClient = null;
        }
    }
}
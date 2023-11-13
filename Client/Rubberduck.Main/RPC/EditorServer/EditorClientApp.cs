using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using Rubberduck.Editor;
using Rubberduck.Editor.RPC;
using Rubberduck.Editor.RPC.LanguageServerClient;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Env = System.Environment;
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
        private IDisposable? _editorClientInitializeTask = default;

        public EditorClientApp(ILogger<EditorClientApp> logger, ServerStartupOptions options, CancellationTokenSource tokenSource, IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _tokenSource = tokenSource;
            _services = services;
        }

        public async Task StartupAsync()
        {
            StartEditorServerProcess();

            _editorClient = await OmniSharpLanguageClient.From(ConfigureClient, _services, _tokenSource.Token);

            _logger.LogTrace("EditorClient configured. Initializing...");
            _editorClientInitializeTask = _editorClient.Initialize(_tokenSource.Token);
        }

        public async Task ExitAsync()
        {
            _editorClient?.SendShutdown(new());
            await SendExitServerNotificationAsync();
        }

        private async Task SendExitServerNotificationAsync()
        {
            var delay = _services.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.ExitNotificationDelay;
            await Task.Delay(delay).ContinueWith(o => _editorClient?.SendExit(new()), _tokenSource.Token, TaskContinuationOptions.None, TaskScheduler.Default).ConfigureAwait(false);
        }

        private void StartEditorServerProcess()
        {
            var settings = _services
                .GetRequiredService<RubberduckSettingsProvider>().Settings
                .LanguageServerSettings.StartupSettings;
            var clientProcessId = Env.ProcessId;

            _editorServerProcess = new LanguageServerProcess(_logger).Start(clientProcessId, settings, HandleServerExit);
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
            switch (_options.TransportType)
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
        }

        private void ConfigureStdIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            _logger.LogInformation($"Configuring editor client transport to use standard input/output streams...");
            var serverProcess = _editorServerProcess ?? throw new InvalidOperationException("BUG: Server process is not initialized.");

            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            EditorClientService.ConfigureEditorClient(Assembly.GetExecutingAssembly(), serverProcess, Env.ProcessId, settings, workspaceRoot.LocalPath);
        }

        private void ConfigurePipeIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            var processId = Env.ProcessId;
            var ioOptions = (PipeServerStartupOptions)_options;

            var pipeName = ioOptions.PipeName ?? PipeServerStartupOptions.GetPipeName(settings.LanguageClientSettings.StartupSettings.ServerPipeName, processId);
            _logger.LogInformation("Configuring editor client transport to use a named pipe stream (name: {pipeName} mode: {mode})...", pipeName, ioOptions.Mode);

            var pipe = _editorClientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
            options.WithInput(pipe.UsePipeReader());
            options.WithOutput(pipe.UsePipeWriter());

            EditorClientService.ConfigureEditorClient(Assembly.GetExecutingAssembly(), pipe, processId, settings, workspaceRoot.LocalPath);
        }

        public void Dispose()
        {
            _editorServerProcess?.Dispose();
            _editorClientPipe?.Dispose();
            _editorClient?.Dispose();
            _editorClientInitializeTask?.Dispose();

            _editorServerProcess = null;
            _editorClientPipe = null;
            _editorClient = null;
            _editorClientInitializeTask = null;
        }
    }
}
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguagePlatform;
using System;
using System.Globalization;
using System.Text.Json;

namespace Rubberduck.ServerPlatform
{
    public abstract class ServerState<TSettings, TStartupSettings> : IServerStateWriter
        where TStartupSettings : IHealthCheckSettingsProvider
    {
        //private readonly ServerStartupOptions _startupOptions;
        private readonly IHealthCheckService<TStartupSettings> _healthCheckService;

        public ServerState(
            ILogger<ServerState<TSettings, TStartupSettings>> logger, 
            //ServerStartupOptions startupOptions,
            IHealthCheckService<TStartupSettings> healthCheck)
        {
            _logger = logger;
            //_startupOptions = startupOptions;
            _healthCheckService = healthCheck;

            _clientInfo = default;
            _capabilities = default;
            _processId = default;
            _locale = default;
            _options = default;
            _traceLevel = default;
        }

        private readonly ILogger _logger;
        protected ILogger Logger => _logger;

        private ClientInfo? _clientInfo;
        public ClientInfo ClientInfo => _clientInfo ?? throw new ServerStateNotInitializedException();

        private ClientCapabilities? _capabilities;
        public ClientCapabilities ClientCapabilities => _capabilities ?? throw new ServerStateNotInitializedException();

        private long? _processId;
        public long ClientProcessId => _processId ?? throw new ServerStateNotInitializedException();

        private InitializationOptions? _options;
        public InitializationOptions Options => _options ?? throw new ServerStateNotInitializedException();

        private CultureInfo? _locale;
        public CultureInfo Locale => _locale ?? throw new ServerStateNotInitializedException();

        private InitializeTrace? _traceLevel;
        public InitializeTrace TraceLevel => _traceLevel ?? throw new ServerStateNotInitializedException();

        public void SetTraceLevel(InitializeTrace value)
        {
            var oldValue = _traceLevel;
            if (_traceLevel != value)
            {
                _traceLevel = value;
                _logger.LogInformation(value.ToTraceLevel(), "Server trace level was changed.", $"OldValue: '{oldValue}' NewValue: '{value}'");
            }
            else if (_traceLevel != null)
            {
                _logger.LogWarning(_traceLevel.Value.ToTraceLevel(), "SetTraceLevel is unchanged.", $"Value: '{value}'");
            }
            else
            {
                throw new ServerStateNotInitializedException();
            }
        }

        public void Initialize(InitializeParams param)
        {
            InvalidInitializeParamsException.ThrowIfNull(param,
                e => (nameof(e.ClientInfo), param.ClientInfo),
                e => (nameof(e.InitializationOptions), param.InitializationOptions),
                e => (nameof(e.Capabilities), param.Capabilities),
                e => (nameof(e.ProcessId), param.ProcessId),
                e => (nameof(e.Trace), param.Trace),
                e => (nameof(e.WorkspaceFolders), param.WorkspaceFolders)
            );

            var options = param.InitializationOptions!.ToString()!;
            _options = JsonSerializer.Deserialize<InitializationOptions>(options);

            _capabilities = param.Capabilities!;
            _clientInfo = param.ClientInfo!;
            _processId = param.ProcessId!.Value;
            _traceLevel = param.Trace!;
            _locale = new CultureInfo(_options.Value.Locale);

            _logger.LogDebug(TraceLevel.ToTraceLevel(), "Received valid initialization options.", options);
            StartClientHealthCheckService();
            _healthCheckService.ChildProcessExited += HandleChildProcessExited;

            OnInitialize(param);
        }

        private void HandleChildProcessExited(object? sender, EventArgs e) => OnClientProcessExited();

        protected virtual void OnInitialize(InitializeParams param)
        {
        }

        protected virtual void OnClientProcessExited()
        {
        }

        public void StartClientHealthCheckService()
        {
            if (TimedAction.TryRun(() =>
            {
                _healthCheckService.Start();

            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(TraceLevel.ToTraceLevel(), "Started healthcheck service.", elapsed);
            }
            else if (exception != null)
            {
                _logger.LogError(TraceLevel.ToTraceLevel(), exception, "Healthcheck service could not be started. This exception will be thrown from this location.");
                throw exception;
            }
        }

        public void Shutdown(ShutdownParams param)
        {
            _ = param ?? throw new ArgumentNullException(nameof(param), "Shutdown state cannot be meaningfully set with a null parameter.");
            _shutdownParams = param;
            _healthCheckService.ChildProcessExited -= HandleChildProcessExited;
            _logger.LogInformation(TraceLevel.ToTraceLevel(), "Shutdown state was set.", $"IsCleanExit: {IsCleanExit}");
        }

        private ShutdownParams? _shutdownParams;
        public bool IsCleanExit => _shutdownParams != null;
    }
}
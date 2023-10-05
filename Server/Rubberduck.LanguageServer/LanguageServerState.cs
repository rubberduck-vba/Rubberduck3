using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Globalization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Collections.Generic;
using Rubberduck.ServerPlatform;
using Rubberduck.InternalApi.Extensions;
using System.Linq;

namespace Rubberduck.LanguageServer
{
    public interface IServerStateWriter
    {
        void Initialize(InitializeParams param);
        void SetTraceLevel(InitializeTrace level);
        void AddWorkspaceFolders(IEnumerable<WorkspaceFolder> workspaceFolders);
    }

    public class LanguageServerState<TOptions> : IServerStateWriter
        where TOptions : new()
    {
        private readonly ServerStartupOptions _startupOptions;

        public LanguageServerState(ILogger logger, ServerStartupOptions startupOptions)
        {
            _logger = logger;
            _startupOptions = startupOptions;

            _clientInfo = default;
            _capabilities = default;
            _processId = default;
            _locale = default;
            _workspaceFolders = default;
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

        private TOptions? _options;
        public TOptions Options => _options ?? throw new ServerStateNotInitializedException();

        private CultureInfo? _locale;
        public CultureInfo Locale => _locale ?? throw new ServerStateNotInitializedException();

        private InitializeTrace? _traceLevel;
        public InitializeTrace TraceLevel => _traceLevel ?? throw new ServerStateNotInitializedException();

        private Container<WorkspaceFolder>? _workspaceFolders;
        public IEnumerable<WorkspaceFolder> Workspacefolders => _workspaceFolders ?? throw new ServerStateNotInitializedException();

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

        public void AddWorkspaceFolders(IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            _workspaceFolders = _workspaceFolders?.Concat(workspaceFolders).ToContainer() ?? throw new ServerStateNotInitializedException();
        }

        public void Initialize(InitializeParams param)
        {
            InvalidInitializeParamsException.ThrowIfNull(param,
                e => (nameof(e.ClientInfo), e.ClientInfo),
                e => (nameof(e.InitializationOptions), e.InitializationOptions),
                e => (nameof(e.Capabilities), e.Capabilities),
                e => (nameof(e.Locale), e.Locale),
                e => (nameof(e.ProcessId), e.ProcessId),
                e => (nameof(e.Trace), e.Trace),
                e => (nameof(e.WorkspaceFolders), e.WorkspaceFolders)
            );

            _logger.LogTrace("Received InitializeParams: {param}", param);

            _capabilities = param.Capabilities!;
            _clientInfo = param.ClientInfo!;
            _locale = CultureInfo.CurrentCulture.FromLocale(param.Locale);
            _processId = param.ProcessId!.Value;
            _traceLevel = param.Trace!;
            _workspaceFolders = param.WorkspaceFolders!;

            _options = (TOptions)(param.InitializationOptions!);
        }
    }
}
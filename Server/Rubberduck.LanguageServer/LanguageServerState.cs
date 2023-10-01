using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Globalization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Collections.Generic;
using Rubberduck.ServerPlatform;

namespace Rubberduck.LanguageServer
{
    public interface IServerStateWriter
    {
        void Initialize(InitializeParams param);
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

        public void Initialize(InitializeParams param)
        {
            //InvalidInitializeParamsException.ThrowIfNull(param,
            //    e => (nameof(e.ClientInfo), e.ClientInfo),
            //    e => (nameof(e.Capabilities), e.Capabilities),
            //    //e => (nameof(e.Locale), e.Locale),
            //    //e => (nameof(e.ProcessId), e.ProcessId),
            //    e => (nameof(e.WorkspaceFolders), e.WorkspaceFolders)
            //);

            _logger.LogTrace("Received InitializeParams: {param}", param);

            _capabilities = param.Capabilities!;
            _clientInfo = param.ClientInfo!;
            //_locale = ToCultureInfo(param.Locale!);
            //_processId = param.ProcessId!.Value;
            _traceLevel = param.Trace!;
            _workspaceFolders = param.WorkspaceFolders!;

            //_options = (TOptions)(param.InitializationOptions ?? new());
        }

        private CultureInfo ToCultureInfo(string? locale)
        {
            try
            {
                if (locale is null)
                {
                    _logger.LogWarning("Could not set locale from initialization parameters.");
                    return CultureInfo.InvariantCulture;
                }

                return CultureInfo.GetCultureInfo(locale);
            }
            catch
            {
                _logger.LogWarning("Could not set locale from initialization parameters.");
                return CultureInfo.InvariantCulture;
            }
        }
    }
}
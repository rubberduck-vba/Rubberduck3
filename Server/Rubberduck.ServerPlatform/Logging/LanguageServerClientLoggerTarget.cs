﻿using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.Extensions;
using System.Collections.Generic;
using NLog;
using NLog.Targets;

namespace Rubberduck.ServerPlatform.Logging
{
    [Target("LSP")]
    public class LanguageServerClientLoggerTarget : TargetWithLayout
    {
        private LanguageServerProvider _provider;

        private static readonly Dictionary<LogLevel, MessageType> _map = new()
        {
            [LogLevel.Trace] = MessageType.Log,
            [LogLevel.Debug] = MessageType.Log,
            [LogLevel.Info] = MessageType.Info,
            [LogLevel.Warn] = MessageType.Warning,
            [LogLevel.Error] = MessageType.Error,
            [LogLevel.Fatal] = MessageType.Error,
        };

        protected override void InitializeTarget()
        {
            _provider = ResolveService<LanguageServerProvider>();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var lsp = _provider?.LanguageServer;
            if (lsp is null || logEvent.Level == LogLevel.Off)
            {
                return;
            }

            if (logEvent.Level == LogLevel.Trace)
            {
                var request = new LogTraceParams
                {
                    Message = logEvent.FormattedMessage,
                };
                lsp.LogTrace(request);
            }
            else
            {
                var request = new LogMessageParams
                {
                    Message = logEvent.FormattedMessage,
                    Type = _map[logEvent.Level]
                };
                lsp.LogMessage(request);
            }
        }
    }
}
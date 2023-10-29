﻿using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Editor.EditorServer;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle
{
    public class ExitHandler : ExitHandlerBase
    {
        /* LSP 3.17 Exit Notification
         * A notification to ask the server to exit its process. 
         * The server should exit with success code 0 if the shutdown request has been received before; otherwise with error code 1.
        */ 

        private readonly ILogger _logger;
        private readonly ISettingsProvider<GeneralSettings> _settingsProvider;
        private readonly Func<EditorServerState> _state;

        public ExitHandler(ILogger<ExitHandler> logger, ISettingsProvider<GeneralSettings> settingsProvider, Func<EditorServerState> state)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _state = state;
        }

        public async override Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Received Exit notification.");

            cancellationToken.ThrowIfCancellationRequested();
            var traceLevel = _settingsProvider.Settings.TraceLevel.ToTraceLevel();

            if (TimedAction.TryRun(() =>
            {
                _logger.LogInformation("Handling exit notification...");
                if (_state().IsCleanExit)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Environment.Exit(1);
                }
            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(traceLevel, "Handled Exit notification. Process should exit with code 0.", elapsed);
            }
            else if (exception != null)
            {
                _logger.LogError(traceLevel, exception);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}
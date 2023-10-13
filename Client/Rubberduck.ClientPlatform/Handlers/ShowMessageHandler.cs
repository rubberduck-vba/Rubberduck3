using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Rubberduck.Resources;
using Rubberduck.InternalApi.Extensions;
using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using System.Diagnostics;
using Rubberduck.UI.Message;
using System.Collections.Generic;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Common;
using System.Linq;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageHandler : ShowMessageHandlerBase
    {
        private readonly ILogger<ShowMessageHandler> _logger;
        private readonly IMessageService _service;
        private readonly ISettingsProvider<RubberduckSettings> _settingsProvider;
        TraceLevel TraceLevel => _settingsProvider.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

        public ShowMessageHandler(ILogger<ShowMessageHandler> logger, IMessageService service, ISettingsProvider<RubberduckSettings> settingsProvider)
        {
            _logger = logger;
            _service = service;
            _settingsProvider = settingsProvider;
        }

        private static readonly IDictionary<MessageType, LogLevel> _typeMap = new Dictionary<MessageType, LogLevel>
        {
            [MessageType.Info] = LogLevel.Information,
            [MessageType.Warning] = LogLevel.Warning,
            [MessageType.Error] = LogLevel.Error
        };

        public override async Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation(TraceLevel, "Handling ShowMessage request.", $"[{request.Type}] {request.Message}");

            if (_typeMap.TryGetValue(request.Type, out var level))
            {
                if (TimedAction.TryRun(() =>
                {
                    var model = MessageModel.FromShowMessageParams(request.Message, level);
                    if (!_settingsProvider.Settings.LanguageClientSettings.DisabledMessageKeys.Contains(model.Key))
                    {
                        var result = _service.ShowMessage(model);

                        if (!result.IsEnabled)
                        {
                            // TODO update settings
                            //_settingsProvider.Settings.LanguageClientSettings.DisabledMessageKeys.Add(model.Key);
                        }
                    }

                }, out var elapsed, out var exception))
                {
                    _logger.LogPerformance(TraceLevel, "ShowMessage request completed.", elapsed);
                }
                else if (exception is not null)
                {
                    _logger.LogError(TraceLevel, exception);
                }
            }
            else
            {
                // MessageType.Log does not pop a message box, but should appear in a server trace toolwindow.
                _logger.LogInformation(TraceLevel, request.Message);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}

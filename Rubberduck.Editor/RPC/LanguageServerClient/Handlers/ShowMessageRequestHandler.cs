using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Rubberduck.InternalApi.Settings;
using Rubberduck.UI;
using Rubberduck.UI.Message;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class ShowMessageRequestHandler : ShowMessageRequestHandlerBase
    {
        private readonly ILogger<ShowMessageRequestHandler> _logger;
        private readonly IMessageService _service;
        private readonly ISettingsProvider<RubberduckSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Settings.LanguageServerSettings.TraceLevel.ToTraceLevel();

        public ShowMessageRequestHandler(ILogger<ShowMessageRequestHandler> logger,
            IMessageService service,
            ISettingsProvider<RubberduckSettings> settingsProvider)
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

        public override async Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation(TraceLevel, "Handling ShowMessageRequest request.", $"[{request.Type}] {request.Message}");

            var result = MessageActionResult.Default;
            if (_typeMap.TryGetValue(request.Type, out var level))
            {
                if (TimedAction.TryRun(() =>
                {
                    var actions = request.Actions?.Select(e => e.ToMessageAction()).ToArray();
                    if (actions is null)
                    {
                        _logger.LogWarning(TraceLevel, "ShowMessageRequestParams did not include any message actions; using default Accept/Cancel actions. This is likely a bug.");
                        actions = new[] { MessageAction.AcceptAction, MessageAction.CancelAction };
                    }

                    var model = MessageRequestModel.For(level, request.Message, actions);
                    if (!_settingsProvider.Settings.LanguageClientSettings.DisabledMessageKeys.Contains(model.Key))
                    {
                        result = _service.ShowMessageRequest(model);

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

            return await Task.FromResult(result.ToMessageActionItem(request));
        }
    }
}

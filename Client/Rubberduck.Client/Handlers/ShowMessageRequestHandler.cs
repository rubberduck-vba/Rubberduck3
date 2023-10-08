using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageRequestHandler : ShowMessageRequestHandlerBase
    {
        private readonly ILogger<ShowMessageRequestHandler> _logger;
        private readonly IMessageBox _service;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();

        public ShowMessageRequestHandler(ILogger<ShowMessageRequestHandler> logger, IMessageBox service, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            _service = service;
            _settingsProvider = settingsProvider;
        }

        public override async Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation(TraceLevel, "Handling ShowMessageRequest request.", $"[{request.Type}] {request.Message}");

            // TODO map request actions to buttons, map result to selected action item... implement a message box with VM-supplied action buttons

            bool confirmed = false;
            switch (request.Type)
            {
                case MessageType.Error:
                case MessageType.Warning:
                    confirmed = _service.ConfirmYesNo(request.Message, RubberduckUI.Rubberduck);
                    break;
                case MessageType.Info:
                case MessageType.Log:
                    confirmed = _service.Question(request.Message, RubberduckUI.Rubberduck);
                    break;
            }

            var response = new MessageActionItem
            {
                Title = confirmed ? "ok" : "cancel"
            };

            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(response);
        }
    }
}

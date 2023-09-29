using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.InternalApi.Extensions;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageRequestHandler : ShowMessageRequestHandlerBase
    {
        private readonly ILogger<ShowMessageRequestHandler> _logger;
        private readonly IMessageBox _service;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Value.Settings.TraceLevel.ToTraceLevel();

        public ShowMessageRequestHandler(ILogger<ShowMessageRequestHandler> logger, IMessageBox service, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            _service = service;
            _settingsProvider = settingsProvider;
        }

        public override async Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Handling ShowMessageRequest request.", $"[{request.Type}] {request.Message}", TraceLevel);

            MessageActionItem response = new MessageActionItem { Title = "OK" };
            // var result = _messageBox.Show(request.Message, request.Type, request.Actions)
            // TODO map actions to buttons, map result to selected action item... implement a message box with VM-supplied action buttons
            
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(response);
        }
    }
}

using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Message;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageRequestHandler : ShowMessageRequestHandlerBase
    {
        private readonly ILogger<ShowMessageRequestHandler> _logger;
        //private readonly IMessageService _service;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();

        public ShowMessageRequestHandler(ILogger<ShowMessageRequestHandler> logger, 
            //IMessageService service, 
            ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            //_service = service;
            _settingsProvider = settingsProvider;
        }

        public override async Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation(TraceLevel, "Handling ShowMessageRequest request.", $"[{request.Type}] {request.Message}");

            // TODO map request actions to buttons, map result to selected action item...

            bool confirmed = false;
            switch (request.Type)
            {
                case MessageType.Error:
                case MessageType.Warning:
                    //confirmed = _service.Show(LogLevel.Error, request.Message, request.Message, RubberduckUI.Rubberduck);
                    break;
                case MessageType.Info:
                case MessageType.Log:
                    //confirmed = _service.Show(LogLevel.Information, request.Message, request.Message, RubberduckUI.Rubberduck, actions:request.Actions);
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

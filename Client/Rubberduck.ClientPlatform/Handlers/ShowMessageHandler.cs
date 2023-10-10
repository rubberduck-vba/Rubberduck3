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

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageHandler : ShowMessageHandlerBase
    {
        private readonly ILogger<ShowMessageHandler> _logger;
        private readonly IMessageService _service;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();

        public ShowMessageHandler(ILogger<ShowMessageHandler> logger, IMessageService service, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            _service = service;
            _settingsProvider = settingsProvider;
        }

        public override async Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation(TraceLevel, "Handling ShowMessage request.", $"[{request.Type}] {request.Message}");

            switch (request.Type)
            {
                case MessageType.Error:
                    _service.ShowError(request.Message, request.Message, RubberduckUI.Rubberduck);
                    break;
                case MessageType.Warning:
                    _service.ShowWarning(request.Message, request.Message, RubberduckUI.Rubberduck);
                    break;
                case MessageType.Info:
                    _service.ShowMessage(request.Message, request.Message, RubberduckUI.Rubberduck);
                    break;
                default:
                    // serverConsole.Log(request.Message, request.Type);
                    break;
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}

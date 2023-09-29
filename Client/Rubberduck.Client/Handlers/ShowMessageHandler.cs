using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.Resources;
using Rubberduck.InternalApi.Extensions;
using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using System.Diagnostics;

namespace Rubberduck.Client.Handlers
{
    public class ShowMessageHandler : ShowMessageHandlerBase
    {
        private readonly ILogger<ShowMessageHandler> _logger;
        private readonly IMessageBox _service;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Value.Settings.TraceLevel.ToTraceLevel();

        public ShowMessageHandler(ILogger<ShowMessageHandler> logger, IMessageBox service, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            _service = service;
            _settingsProvider = settingsProvider;
        }

        public override async Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Handling ShowMessage request.", $"[{request.Type}] {request.Message}", TraceLevel);

            switch (request.Type)
            {
                case MessageType.Error:
                    _service.NotifyError(request.Message, RubberduckUI.Rubberduck, string.Empty);
                    break;
                case MessageType.Warning:
                    _service.NotifyWarn(request.Message, RubberduckUI.Rubberduck);
                    break;
                case MessageType.Info:
                    _service.Message(request.Message);
                    break;
                default:
                    // serverConsole.Log(request.Message, request.Type);
                    break;
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}

using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class LogMessageHandler : LogMessageHandlerBase
    {
        private readonly ILogger<WorkspaceFoldersHandler> _logger;

        public LogMessageHandler(ILogger<WorkspaceFoldersHandler> logger, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
        }

        public override async Task<Unit> Handle(LogMessageParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // _serverConsole.LogMessage(request.Message, request.Type)

            return await Task.FromResult(Unit.Value);
        }
    }
}

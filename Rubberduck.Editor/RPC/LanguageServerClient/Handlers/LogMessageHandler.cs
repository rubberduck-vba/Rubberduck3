using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Rubberduck.SettingsProvider.Model.LanguageServer;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class ExecuteCommandHandler : ExecuteCommandHandlerBase
    {
        public override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities)
        {
            return new()
            {
                WorkDoneProgress = true,
                Commands = new Container<string>(
                    "CreateProjectWorkspace"
                )
            };
        }
    }

    public class LogMessageHandler : LogMessageHandlerBase
    {
        private readonly ILogger<WorkspaceFoldersHandler> _logger;

        public LogMessageHandler(ILogger<WorkspaceFoldersHandler> logger, ISettingsProvider<LanguageServerSettingsGroup> settingsProvider)
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

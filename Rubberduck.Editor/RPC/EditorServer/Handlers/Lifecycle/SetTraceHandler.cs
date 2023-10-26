using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.ServerPlatform;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle
{
    public class SetTraceHandler : SetTraceHandlerBase
    {
        private readonly ILogger _logger;
        private readonly IServerStateWriter _state;

        public SetTraceHandler(ILogger<SetTraceHandler> logger, IServerStateWriter state)
        {
            _logger = logger;
            _state = state;
        }

        public async override Task<Unit> Handle(SetTraceParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Received SetTrace request.");

            _state.SetTraceLevel(request.Value);

            _logger.LogDebug("Completed SetTrace request.");
            return await Task.FromResult(Unit.Value);
        }
    }
}
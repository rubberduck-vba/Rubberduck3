using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.TelemetryServer.Handlers.Lifecycle
{
    public class InitializedHandler : LanguageProtocolInitializedHandlerBase
    {
        private readonly ILogger<InitializedHandler> _logger;

        public InitializedHandler(ILogger<InitializedHandler> logger)
        {
            _logger = logger;
        }

        public async override Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initialized.");
            return await Task.FromResult(Unit.Value);
        }
    }
}
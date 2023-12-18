using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Rubberduck.ServerPlatform;
using System;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class WorkDoneProgressHandler : ProgressHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;

        public WorkDoneProgressHandler(ServerPlatformServiceHelper service)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var token = request.Token;
            var value = request.Value.ToObject<WorkDoneProgressReport>();
            
            _service.OnProgress(token, value);
            return await Task.FromResult(Unit.Value);
        }
    }
}

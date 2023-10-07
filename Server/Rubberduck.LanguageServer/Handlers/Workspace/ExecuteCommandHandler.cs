using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class ExecuteCommandHandler : ExecuteCommandHandlerBase
    {
        public async override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO

            return await Task.FromResult(Unit.Value);
        }

        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities)
        {
            return new ExecuteCommandRegistrationOptions
            {
                WorkDoneProgress = true,
                Commands = new Container<string>(/* TODO */)
            };
        }
    }
}
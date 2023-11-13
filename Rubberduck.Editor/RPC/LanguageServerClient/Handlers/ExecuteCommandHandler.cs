using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

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
                )
            };
        }
    }
}

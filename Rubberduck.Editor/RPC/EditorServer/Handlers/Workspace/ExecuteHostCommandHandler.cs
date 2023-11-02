using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace
{
    public class ExecuteHostCommandHandler : AbstractHandlers.Request<ExecuteCommandParams, MediatR.Unit, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>, IExecuteCommandHandler
    {
        public override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities)
        {
            throw new NotImplementedException();
        }
    }
}
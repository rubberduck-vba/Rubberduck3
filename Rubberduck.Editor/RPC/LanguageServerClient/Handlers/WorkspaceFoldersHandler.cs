using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.ServerPlatform;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class WorkspaceFoldersHandler : WorkspaceFoldersHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        public WorkspaceFoldersHandler(ServerPlatformServiceHelper service)
        {
            _service = service;
        }

        public override async Task<Container<WorkspaceFolder>?> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Container<WorkspaceFolder> response = null!;

            try
            {
                _service.RunAction(() =>
                    {
                        var workspaceFolders = Enumerable.Empty<WorkspaceFolder>(); // TODO
                        response = new Container<WorkspaceFolder>(workspaceFolders);
                    }, nameof(WorkspaceFoldersHandler));
            }
            catch(Exception exception)
            {
                throw new RequestFailedException(nameof(WorkspaceFoldersHandler), exception);
            }

            return await Task.FromResult(response);
        }
    }
}

using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.ServerPlatform;
using System.Linq;

namespace Rubberduck.Main.RPC.EditorServer.Handlers
{
    public class WorkspaceFoldersHandler : WorkspaceFoldersHandlerBase
    {
        /**
         * EditorServer sends this request once started to load additional workspaces,
         * for example if multiple unlocked projects are loaded in the VBE.
         * Use this handler to provide the language server with all the projects loaded in the VBE,
         * irrespective of whether or not they're referenced by the project the server was initialized for.
        **/

        /** implementation notes and considerations:
         * Server (Rubberduck.Editor) will ignore the workspace it was initialized with;
         * therefore, simply return an empty container if there's only a single project/document in the VBE/host.
        */

        private readonly ServerPlatformServiceHelper _service;

        public WorkspaceFoldersHandler(ServerPlatformServiceHelper service)
        {
            _service = service;
        }

        public async override Task<Container<WorkspaceFolder>?> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken)
        {
            var folders = Enumerable.Empty<WorkspaceFolder>();

            //_service.TryRunAction(() =>
            //{
            //    // TODO
            //});

            return await Task.FromResult(new Container<WorkspaceFolder>(folders));
        }
    }
}

using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Main.RPC.EditorServer.Handlers
{
    public static class DocumentChangesExtensions
    {
        public static IDictionary<DocumentUri, List<WorkspaceEditDocumentChange>> GroupByDocumentUri(this Container<WorkspaceEditDocumentChange> changes)
        {
            var edits = new Dictionary<DocumentUri, List<WorkspaceEditDocumentChange>>();

            foreach (var change in changes)
            {
                if (change.IsTextDocumentEdit && change.TextDocumentEdit is not null)
                {
                    var uri = change.TextDocumentEdit.TextDocument.Uri;
                    if (!edits.TryGetValue(uri, out var knownEdits))
                    {
                        edits.Add(uri, new List<WorkspaceEditDocumentChange> { change });
                    }
                    else
                    {
                        if (!knownEdits.Any(e => e.IsDeleteFile || e.IsRenameFile))
                        {
                            edits[uri].Add(change);
                        }
                    }
                }
                else if (change.IsRenameFile && change.RenameFile is not null)
                {
                    var uri = change.RenameFile.OldUri;
                    if (!edits.ContainsKey(uri))
                    {
                        edits.Add(uri, new List<WorkspaceEditDocumentChange> { change });
                    }
                    else
                    {
                        edits[uri] = new List<WorkspaceEditDocumentChange> { change };  // overwrites previous edits to this uri
                    }
                }
                else if (change.IsCreateFile && change.CreateFile is not null)
                {
                    var uri = change.CreateFile.Uri;
                    if (!edits.ContainsKey(uri))
                    {
                        edits.Add(uri, new List<WorkspaceEditDocumentChange> { change });
                    }
                    else
                    {
                        edits[uri].Add(change);
                    }
                }
                else if (change.IsDeleteFile && change.DeleteFile is not null)
                {
                    var uri = change.DeleteFile.Uri;
                    if (!edits.ContainsKey(uri))
                    {
                        edits.Add(uri, new List<WorkspaceEditDocumentChange> { change });
                    }
                    else
                    {
                        edits[uri] = new List<WorkspaceEditDocumentChange> { change }; // overwrites previous edits to this uri
                    }
                }
            }
            return edits;
        }
    }

    /// <summary>
    /// The VBE handles workspace edits sent from the RDE by synchronizing the specified documents from the workspace into the VBE.
    /// </summary>
    public class ApplyWorkspaceEditHandler : ApplyWorkspaceEditHandlerBase
    {
        public override async Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken)
        {
            /**
             * Depending on the client capability
             * `workspace.workspaceEdit.resourceOperations` document changes are either
             * an array of `TextDocumentEdit`s to express changes to n different text
             * documents where each text document edit addresses a specific version of
             * a text document. Or it can contain above `TextDocumentEdit`s mixed with
             * create, rename and delete file / folder operations.
             *
             * Whether a client supports versioned document edits is expressed via
             * `workspace.workspaceEdit.documentChanges` client capability.
             *
             * If a client neither supports `documentChanges` nor
             * `workspace.workspaceEdit.resourceOperations` then only plain `TextEdit`s
             * using the `changes` property are supported.
             * https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#workspaceEdit
             */

            cancellationToken.ThrowIfCancellationRequested();

            var synchronized = new HashSet<DocumentUri>();

            if (request.Edit.DocumentChanges is not null)
            {
                var changes = request.Edit.DocumentChanges.GroupByDocumentUri();
                foreach (var uri in changes.Keys)
                {
                    foreach (var change in changes[uri])
                    {
                        if (change.IsTextDocumentEdit && !synchronized.Contains(uri))
                        {
                            var path = uri.Path;
                            // TODO sync VBE from file

                            synchronized.Add(uri);
                            break;
                        }
                        else if (change.IsRenameFile && !synchronized.Contains(uri))
                        {
                            // TODO remove old-name module from VBA project
                            break;
                        }
                        else if (change.IsCreateFile && !synchronized.Contains(uri))
                        {

                            break;
                        }
                    }
                }
            }
            else if (request.Edit.Changes is not null)
            {
                foreach (var uri in request.Edit.Changes.Keys)
                {
                    var path = uri.Path;
                    // TODO sync VBE from file

                    synchronized.Add(uri);
                }
            }


            var response = new ApplyWorkspaceEditResponse
            { 
                Applied = true,
                
            };
            // TODO
            /*
             * The workspace/applyEdit request is sent from the server to the client to modify resource on the client side.
            */

            return await Task.FromResult(response);
        }
    }
}

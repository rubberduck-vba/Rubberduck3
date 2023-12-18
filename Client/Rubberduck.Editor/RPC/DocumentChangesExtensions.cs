using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Editor.RPC
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
}

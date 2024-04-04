using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Main.RPC.EditorServer.Handlers
{
    public static class DocumentChangesExtensions
    {
        public static IDictionary<DocumentUri, List<(WorkspaceEditDocumentChange, int)>> GroupByDocumentUri(this Container<WorkspaceEditDocumentChange> changes)
        {
            var edits = new Dictionary<DocumentUri, List<(WorkspaceEditDocumentChange, int)>>();

            foreach (var indexedChange in changes.Select((e, i) => (Change:e, Index:i)))
            {
                if (indexedChange.Change.IsTextDocumentEdit && indexedChange.Change.TextDocumentEdit is not null)
                {
                    var uri = indexedChange.Change.TextDocumentEdit.TextDocument.Uri;
                    if (!edits.TryGetValue(uri, out var knownEdits))
                    {
                        edits.Add(uri, new List<(WorkspaceEditDocumentChange, int)> { indexedChange });
                    }
                    else
                    {
                        if (!knownEdits.Any(e => e.Item1.IsDeleteFile || e.Item1.IsRenameFile))
                        {
                            edits[uri].Add(indexedChange);
                        }
                    }
                }
                else if (indexedChange.Change.IsRenameFile && indexedChange.Change.RenameFile is not null)
                {
                    var uri = indexedChange.Change.RenameFile.OldUri;
                    if (!edits.ContainsKey(uri))
                    {
                        edits.Add(uri, new List<(WorkspaceEditDocumentChange, int)> { indexedChange });
                    }
                    else
                    {
                        edits[uri] = new List<(WorkspaceEditDocumentChange, int)> { indexedChange };  // overwrites previous edits to this uri
                    }
                }
                else if (indexedChange.Change.IsCreateFile && indexedChange.Change.CreateFile is not null)
                {
                    var uri = indexedChange.Change.CreateFile.Uri;
                    if (!edits.ContainsKey(uri))
                    {
                        edits.Add(uri, new List<(WorkspaceEditDocumentChange, int)> { indexedChange });
                    }
                    else
                    {
                        edits[uri].Add(indexedChange);
                    }
                }
                else if (indexedChange.Change.IsDeleteFile && indexedChange.Change.DeleteFile is not null)
                {
                    var uri = indexedChange.Change.DeleteFile.Uri;
                    if (!edits.ContainsKey(uri))
                    {
                        edits.Add(uri, new List<(WorkspaceEditDocumentChange, int)> { indexedChange });
                    }
                    else
                    {
                        edits[uri] = new List<(WorkspaceEditDocumentChange, int)> { indexedChange }; // overwrites previous edits to this uri
                    }
                }
            }
            return edits;
        }
    }
}

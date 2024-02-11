using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System.Collections.Concurrent;

namespace Rubberduck.Parsing._v3.Pipeline.Abstract;

public record class ParserPipelineState
{
    public WorkspaceUri WorkspaceRootUri { get; init; } = null!;
    public ConcurrentDictionary<WorkspaceFileUri, DocumentState> Documents { get; init; } = new();
}

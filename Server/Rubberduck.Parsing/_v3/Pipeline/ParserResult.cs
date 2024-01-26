using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public class ParserResult
{
    public WorkspaceFileUri Uri { get; init; } = null!;
    public ParseResult ParseResult { get; init; } = null!;
    public IEnumerable<FoldingRange> Foldings { get; init; } = [];
}

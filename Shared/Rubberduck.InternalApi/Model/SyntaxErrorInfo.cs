using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Parsing.Model;

public record class SyntaxErrorInfo
{
    public WorkspaceFileUri Uri { get; init; } = default!;
    public Range Range { get; init; } = default!;
    public string Message { get; init; } = default!;
}
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;

namespace Rubberduck.InternalApi.Model;

public record class SyntaxErrorInfo
{
    public WorkspaceFileUri Uri { get; init; } = default!;
    public Range Range { get; init; } = default!;
    public string Message { get; init; } = default!;
}
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.InternalApi.Model;

public record class SyntaxErrorInfo
{
    public SyntaxErrorOffendingSymbol Symbol { get; set; } = default!;
    public WorkspaceFileUri Uri { get; init; } = default!;
    public Range Range { get; init; } = default!;
    public string Message { get; init; } = default!;
}
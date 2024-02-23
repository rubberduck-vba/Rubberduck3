using Antlr4.Runtime;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions;

public record class AntlrSyntaxErrorInfo
{
    public WorkspaceFileUri Uri { get; init; } = default!;

    public string Message { get; init; } = default!;

    public RecognitionException Exception { get; init; } = default!;
    public IToken OffendingSymbol { get; init; } = default!;

    public int LineNumber { get; init; }
    public int Position { get; init; }

    public OmniSharp.Extensions.LanguageServer.Protocol.Models.Range Range() =>
        new(start: new Position(OffendingSymbol.Line, OffendingSymbol.Column), 
            end: new Position(OffendingSymbol.EndLine(), OffendingSymbol.EndColumn()));

    public SyntaxErrorInfo ToSyntaxErrorInfo() => new() 
    {
        Uri = Uri,
        Message = Message,
        Range = Range(),
    };
}
using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;
using System.Collections.Immutable;

namespace Rubberduck.Parsing.Exceptions;

public interface ISyntaxErrorMessageService
{
    bool TryGetMeaningfulMessage(AntlrSyntaxErrorInfo info, out string message);
}

public abstract class RubberduckParseErrorListenerBase : BaseErrorListener, IRubberduckParseErrorListener
{
    private readonly ISyntaxErrorMessageService? _errorMessageService;

    public RubberduckParseErrorListenerBase(WorkspaceFileUri uri, ISyntaxErrorMessageService? messageService)
    {
        Uri = uri;
        _errorMessageService = messageService;
    }

    protected WorkspaceFileUri Uri { get; }

    public ImmutableArray<AntlrSyntaxErrorInfo> SyntaxErrors => Errors.ToImmutableArray();

    protected abstract List<AntlrSyntaxErrorInfo> Errors { get; }

    protected virtual AntlrSyntaxErrorInfo GetErrorInfo(IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => new()
    {
        Uri = this.Uri,

        Message = msg,
        Exception = e,
        OffendingSymbol = offendingSymbol,

        LineNumber = line,
        Position = charPositionInLine,
    };


    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var info = GetErrorInfo(offendingSymbol, line, charPositionInLine, msg, e);
        if (_errorMessageService != null && _errorMessageService.TryGetMeaningfulMessage(info, out var message))
        {
            info = info with { Message = message };
        }
        Errors.Add(info);
    }
}

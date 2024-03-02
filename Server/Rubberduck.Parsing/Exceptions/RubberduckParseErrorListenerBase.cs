using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
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
    private readonly bool _throwSyntaxErrors;
    private readonly PredictionMode _mode;

    public RubberduckParseErrorListenerBase(WorkspaceFileUri uri, ISyntaxErrorMessageService? messageService, PredictionMode mode, bool throwOnSyntaxError = false)
    {
        Uri = uri;
        _errorMessageService = messageService;
        _throwSyntaxErrors = throwOnSyntaxError;
        _mode = mode;
    }

    protected WorkspaceFileUri Uri { get; }

    public ImmutableArray<AntlrSyntaxErrorInfo> SyntaxErrors => Errors.ToImmutableArray();

    protected List<AntlrSyntaxErrorInfo> Errors { get; } = [];

    protected virtual AntlrSyntaxErrorInfo GetErrorInfo(IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => new()
    {
        Uri = this.Uri,
        PredictionMode = _mode,

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

        if (_throwSyntaxErrors)
        {
            if (_mode == PredictionMode.Sll)
            {
                Errors.Add(info);
                throw new SllPredictionFailException(info);
            }
            else
            {
                Errors.Add(info);
                throw new SyntaxErrorException(info);
            }
        }
    }
}

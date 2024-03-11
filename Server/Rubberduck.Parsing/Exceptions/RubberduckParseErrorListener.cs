using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.Parsing.Abstract;
using System.Collections.Immutable;

namespace Rubberduck.Parsing.Exceptions;

public interface ISyntaxErrorMessageService
{
    bool TryGetMeaningfulMessage(AntlrSyntaxErrorInfo info, out string message);
}

public class RubberduckParseErrorListener : BaseErrorListener, IRubberduckParseErrorListener
{
    private readonly ISyntaxErrorMessageService? _errorMessageService;
    private readonly bool _throwSyntaxErrors;
    private readonly PredictionMode _mode;

    public RubberduckParseErrorListener(WorkspaceFileUri uri, ISyntaxErrorMessageService? messageService, PredictionMode mode, bool throwOnSyntaxError = false, SllPredictionFailException? sllException = null)
    {
        Uri = uri;
        _errorMessageService = messageService;
        _throwSyntaxErrors = throwOnSyntaxError;
        _mode = mode;

        _errors = sllException is SllPredictionFailException sllFailure 
            ? new() { [sllFailure.OffendingSymbol.TokenIndex] = sllFailure } 
            : [];
    }

    protected WorkspaceFileUri Uri { get; }

    public ImmutableArray<SyntaxErrorException> SyntaxErrors => _errors.Values.ToImmutableArray();

    /// <summary>
    /// Errors by token; LL syntax errors overwrite SLL failures.
    /// </summary>
    /// <remarks>
    /// Key is <c>IToken.TokenIndex</c> from the <c>offendingSymbol</c>.
    /// </remarks>
    private readonly Dictionary<int, SyntaxErrorException> _errors;

    protected void AddOrReplaceError(IToken offendingSymbol, SyntaxErrorException error) => 
        _errors[offendingSymbol.TokenIndex] = error;

    protected virtual AntlrSyntaxErrorInfo GetErrorInfo(IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e) => 
        new()
        {
            Uri = this.Uri,
            PredictionMode = _mode,
        
            Message = msg,
            OffendingSymbol = offendingSymbol,

            LineNumber = line,
            Position = charPositionInLine,

            Exception = e,
        };

    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var message = msg;
        var info = GetErrorInfo(offendingSymbol, line, charPositionInLine, msg, e);
        _ = _errorMessageService?.TryGetMeaningfulMessage(info, out message);

        var symbol = new SyntaxErrorOffendingSymbol(offendingSymbol.Text, Uri)
        {
            Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
            {
                Start = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Position
                {
                    Line = line,
                    Character = charPositionInLine
                },
                End = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Position
                {
                    Line = line,
                    Character = charPositionInLine
                }
            },
            SelectionRange = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
            {
                Start = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Position
                {
                    Line = offendingSymbol.Line,
                    Character = offendingSymbol.Column
                },
                End = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Position
                {
                    Line = offendingSymbol.EndLine(),
                    Character = offendingSymbol.EndColumn()
                }
            }
        };
        var exception = _mode == PredictionMode.Sll
            ? new SllPredictionFailException(Uri, symbol, message, e)
            : new SyntaxErrorException(Uri, symbol, message, e);

        AddOrReplaceError(offendingSymbol, exception);

        if (_throwSyntaxErrors)
        {
            throw exception;
        }
    }
}

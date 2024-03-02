using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Abstract;

public abstract class TokenStreamParserBase<TParser> : ServiceBase, ITokenStreamParser
    where TParser : Parser
{
    private readonly ISyntaxErrorMessageService _errorMessageService;

    protected TokenStreamParserBase(ISyntaxErrorMessageService errorMessageService,
        ILogger<ITokenStreamParser> logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance) 
        : base(logger, settingsProvider, performance)
    {
        _errorMessageService = errorMessageService;
    }

    protected IParseTree Parse(ITokenStream tokenStream, PredictionMode predictionMode, IParserErrorListener? errorListener, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        var parser = GetParser(tokenStream);
        parser.Interpreter.PredictionMode = predictionMode;
        if (errorListener != null)
        {
            parser.AddErrorListener(errorListener);
        }
        if (parseListeners != null)
        {
            foreach (var listener in parseListeners)
            {
                parser.AddParseListener(listener);
            }
        }
        return Parse(parser);
    }
    protected abstract TParser GetParser(ITokenStream tokenStream);
    protected abstract IParseTree Parse(TParser parser);
    public IParseTree Parse(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token, out IEnumerable<SyntaxErrorException> errors, out IEnumerable<Diagnostic> diagnostics, ParserMode parserMode = ParserMode.FallBackSllToLl, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        RubberduckParseErrorListenerBase errorListener;
        var tree = parserMode switch
        {
            ParserMode.FallBackSllToLl => ParseWithFallBack(uri, tokenStream, out errorListener, parseListeners),
            ParserMode.LlOnly => ParseLl(uri, tokenStream, out errorListener, parseListeners),
            ParserMode.SllOnly => ParseSll(uri, tokenStream, out errorListener, parseListeners),
            _ => throw new ArgumentException($"Value '{parserMode}' is not supported.", nameof(parserMode)),
        };

        errors = errorListener.SyntaxErrors
            .Where(e => e is not SllPredictionFailException)
            .ToArray();
        
        diagnostics = errorListener.SyntaxErrors
            .OfType<SllPredictionFailException>()
            .Select(e => e.ToDiagnostic())
            .ToArray();
        
        return tree;
    }

    private IParseTree ParseWithFallBack(WorkspaceFileUri uri, CommonTokenStream tokenStream, out RubberduckParseErrorListenerBase errorListener, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        try
        {
            return ParseSll(uri, tokenStream, out errorListener, parseListeners);
        }
        catch (SllPredictionFailException syntaxErrorException)
        {
            var message = $"SLL mode failed while parsing document (URI: {uri}) at symbol {syntaxErrorException.OffendingSymbol.Text} at L{syntaxErrorException.LineNumber}C{syntaxErrorException.Position}. Retrying using LL prediction mode.";

            LogAndReset(tokenStream, message, syntaxErrorException);
            return ParseLl(uri, tokenStream, out errorListener, parseListeners, syntaxErrorException);
        }
        catch (Exception exception)
        {
            var message = $"SLL mode failed while parsing document (URI {uri}). Retrying using LL prediction mode, but this would be a grammar bug, or a legitimate syntax error.";

            LogAndReset(tokenStream, message, exception);
            return ParseLl(uri, tokenStream, out errorListener, parseListeners, exception);
        }
    }

    //This method is virtual only because a CommonTokenStream cannot be mocked in tests
    //and there is no interface for it or a base class that has the Reset member.
    protected virtual void LogAndReset(CommonTokenStream tokenStream, string logWarnMessage, Exception exception)
    {
        LogWarning(logWarnMessage, verbose: exception.Message);
        tokenStream.Reset();
    }

    private IParseTree ParseLl(WorkspaceFileUri uri, ITokenStream tokenStream, out RubberduckParseErrorListenerBase errorListener, IEnumerable<IParseTreeListener>? parseListeners = null, Exception? sllFailException = null)
    {
        errorListener = new ReportingSyntaxErrorListener(uri, _errorMessageService, PredictionMode.Ll, sllFailException);
        return Parse(tokenStream, PredictionMode.Ll, errorListener, parseListeners);
    }

    private IParseTree ParseSll(WorkspaceFileUri uri, ITokenStream tokenStream, out RubberduckParseErrorListenerBase errorListener, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        errorListener = new ThrowingSyntaxErrorListener(uri, _errorMessageService, PredictionMode.Sll);
        return Parse(tokenStream, PredictionMode.Sll, errorListener, parseListeners);
    }
}

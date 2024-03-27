using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Parsing.Exceptions;

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
        RubberduckParseErrorListener errorListener;
        var tree = parserMode switch
        {
            ParserMode.FallBackSllToLl => ParseWithFallBack(uri, tokenStream, out errorListener, parseListeners),
            ParserMode.LlOnly => ParseLl(uri, tokenStream, out errorListener, parseListeners),
            ParserMode.SllOnly => ParseSll(uri, tokenStream, out errorListener, parseListeners),
            _ => throw new ArgumentException($"Value '{parserMode}' is not supported.", nameof(parserMode)),
        };

        var offendingSymbolErrors = errorListener.SyntaxErrors;

        var syntaxErrors = offendingSymbolErrors.Except(offendingSymbolErrors.OfType<SllPredictionFailException>());
        var sllErrors = offendingSymbolErrors.OfType<SllPredictionFailException>();

        diagnostics = sllErrors.Select(e => e.ToDiagnostic()).ToArray();
        errors = syntaxErrors.ToArray();

        return tree;
    }

    private IParseTree ParseWithFallBack(WorkspaceFileUri uri, CommonTokenStream tokenStream, out RubberduckParseErrorListener errorListener, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        try
        {
            return ParseSll(uri, tokenStream, out errorListener, parseListeners);
        }
        catch (SllPredictionFailException syntaxErrorException)
        {
            var message = $"SLL mode failed while parsing document (URI: {uri}) at symbol {syntaxErrorException.OffendingSymbol.Name} at L{syntaxErrorException.OffendingSymbol.Range.Start.Line}C{syntaxErrorException.OffendingSymbol.Range.Start.Character}. Retrying using LL prediction mode.";

            LogAndReset(tokenStream, message, syntaxErrorException);
            return ParseLl(uri, tokenStream, out errorListener, parseListeners, syntaxErrorException);
        }
        catch (Exception exception)
        {
            var message = $"SLL mode failed while parsing document (URI {uri}). Retrying using LL prediction mode, but this would be a grammar bug, or a legitimate syntax error.";

            LogAndReset(tokenStream, message, exception);
            return ParseLl(uri, tokenStream, out errorListener, parseListeners);
        }
    }

    //This method is virtual only because a CommonTokenStream cannot be mocked in tests
    //and there is no interface for it or a base class that has the Reset member.
    protected virtual void LogAndReset(CommonTokenStream tokenStream, string logWarnMessage, Exception exception)
    {
        LogWarning(logWarnMessage, verbose: exception.Message);
        tokenStream.Reset();
    }

    private IParseTree ParseLl(WorkspaceFileUri uri, ITokenStream tokenStream, out RubberduckParseErrorListener errorListener, IEnumerable<IParseTreeListener>? parseListeners = null, SllPredictionFailException? sllFailException = null)
    {
        errorListener = new RubberduckParseErrorListener(uri, _errorMessageService, PredictionMode.Ll, throwOnSyntaxError: false, sllException: sllFailException);
        return Parse(tokenStream, PredictionMode.Ll, errorListener, parseListeners);
    }

    private IParseTree ParseSll(WorkspaceFileUri uri, ITokenStream tokenStream, out RubberduckParseErrorListener errorListener, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        errorListener = new RubberduckParseErrorListener(uri, _errorMessageService, PredictionMode.Sll, throwOnSyntaxError: true);
        return Parse(tokenStream, PredictionMode.Sll, errorListener, parseListeners);
    }
}

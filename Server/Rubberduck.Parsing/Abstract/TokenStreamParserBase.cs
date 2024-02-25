using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
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
    public IParseTree Parse(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token, out IEnumerable<SyntaxErrorInfo> errors, ParserMode parserMode = ParserMode.FallBackSllToLl, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        var (tree, syntaxErrors) = parserMode switch
        {
            ParserMode.FallBackSllToLl => ParseWithFallBack(uri, tokenStream, parseListeners),
            ParserMode.LlOnly => ParseLl(uri, tokenStream, parseListeners),
            ParserMode.SllOnly => ParseSll(uri, tokenStream, parseListeners),
            _ => throw new ArgumentException($"Value '{parserMode}' is not supported.", nameof(parserMode)),
        };
        errors = syntaxErrors.SyntaxErrors.Select(e => e.ToSyntaxErrorInfo()).ToArray();
        return tree;
    }

    private (IParseTree, RubberduckParseErrorListenerBase) ParseWithFallBack(WorkspaceFileUri uri, CommonTokenStream tokenStream, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        try
        {
            return ParseSll(uri, tokenStream, parseListeners);
        }
        catch (SyntaxErrorException syntaxErrorException)
        {
            var message = $"SLL mode failed while parsing document (URI: {uri}) at symbol {syntaxErrorException.OffendingSymbol.Text} at L{syntaxErrorException.LineNumber}C{syntaxErrorException.Position}. Retrying using LL prediction mode.";
            LogAndReset(tokenStream, message, syntaxErrorException);
            return ParseLl(uri, tokenStream, parseListeners);
        }
        catch (Exception exception)
        {
            var message = $"SLL mode failed while parsing document (URI {uri}). Retrying using LL prediction mode.";
            LogAndReset(tokenStream, message, exception);
            return ParseLl(uri, tokenStream, parseListeners);
        }
    }

    //This method is virtual only because a CommonTokenStream cannot be mocked in tests
    //and there is no interface for it or a base class that has the Reset member.
    protected virtual void LogAndReset(CommonTokenStream tokenStream, string logWarnMessage, Exception exception)
    {
        LogWarning(logWarnMessage, verbose: exception.Message);
        tokenStream.Reset();
    }

    private (IParseTree, RubberduckParseErrorListenerBase) ParseLl(WorkspaceFileUri uri, ITokenStream tokenStream, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        var errorListener = new ReportingSyntaxErrorListener(uri, _errorMessageService);
        var tree = Parse(tokenStream, PredictionMode.Ll, errorListener, parseListeners);
        return (tree, errorListener);
    }

    private (IParseTree, RubberduckParseErrorListenerBase) ParseSll(WorkspaceFileUri uri, ITokenStream tokenStream, IEnumerable<IParseTreeListener>? parseListeners = null)
    {
        //var errorListener = new ThrowingSyntaxErrorListener(uri, _errorMessageService);
        var errorListener = new ReportingSyntaxErrorListener(uri, _errorMessageService);
        var tree = Parse(tokenStream, PredictionMode.Sll, errorListener, parseListeners);
        return (tree, errorListener);
    }
}

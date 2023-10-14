using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.TokenStreamProviders;

namespace Rubberduck.Parsing.Parsers;

public delegate IParseTree ParserStartRule(VBAParser parser);

/// <summary>
/// The class is meant to be used for where we need to do rewriting without actually
/// rewriting an existing module in a VBA project. The result is completely free-floating
/// and it is the caller's responsibility to persist the finalized results into one of
/// actual code. This is useful for e.g. creating a code preview for refactoring.
/// </summary>
public static class VBACodeStringParser
{
    //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// Parses the supplied code.
    /// </summary>
    /// <param name="code">The code to parse.</param>
    /// <param name="startRule">The parser rule to begin the process with.</param>
    /// <param name="mode">(Optional) The parser mode to run.</param>
    public static (IParseTree parseTree, TokenStreamRewriter rewriter) Parse(string code, ParserStartRule startRule, ParserMode mode = ParserMode.FallBackSllToLl)
    {
        var tokenStreamProvider = new StringTokenStreamProvider();
        var tokenStream = tokenStreamProvider.Tokens(code ?? string.Empty);
        
        var parser = new VBAParser(tokenStream);
        try
        {
            return ParseInternal(ParserMode.SllOnly, parser, tokenStream, startRule);
        }
        catch (ParsePassSyntaxErrorException exception)
        {
            var actualMode = parser.Interpreter.PredictionMode.ToString().ToUpperInvariant();
            System.Diagnostics.Debug.Assert(actualMode == "SLL");

            var message = $"{actualMode} mode failed while parsing the code at symbol {exception.OffendingSymbol.Text} at L{exception.LineNumber}C{exception.Position}. Retrying using LL.";
            LogAndReset(message, exception, parser, tokenStream);

            if (parser.Interpreter.PredictionMode == PredictionMode.Sll)
            {
                return ParseInternal(ParserMode.LlOnly, parser, tokenStream, startRule);
            }
        }
        catch (Exception exception)
        {
            var actualMode = parser.Interpreter.PredictionMode.ToString().ToUpperInvariant();
            System.Diagnostics.Debug.Assert(actualMode == "SLL");

            var message = $"{actualMode} mode threw an exception. Retrying LL mode.";
            LogAndReset(message, exception, parser, tokenStream);

            if (parser.Interpreter.PredictionMode == PredictionMode.Sll)
            {
                return ParseInternal(ParserMode.LlOnly, parser, tokenStream, startRule);
            }
        }

        return (null, null);
    }

    private static (IParseTree parseTree, TokenStreamRewriter rewriter) ParseInternal(ParserMode mode, VBAParser parser, CommonTokenStream tokenStream, ParserStartRule startRule)
    {
        if (mode == ParserMode.LlOnly)
        {
            parser.Interpreter.PredictionMode = PredictionMode.Ll;
        }
        else
        {
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
        }
        var tree = startRule.Invoke(parser);
        return (tree, new TokenStreamRewriter(tokenStream));
    }

    private static void LogAndReset(string logWarnMessage, Exception exception, VBAParser parser, CommonTokenStream tokenStream)
    {
        tokenStream.Reset();
        parser.Reset();
    }
}

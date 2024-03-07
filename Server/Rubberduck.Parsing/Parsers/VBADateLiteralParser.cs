using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Parsers;

public sealed class VBADateLiteralParser
{
    /// <summary>
    /// Parses the given date.
    /// </summary>
    /// <param name="date">The date in string format including "hash tags" (e.g. #01-01-1900#)</param>
    /// <returns>The root of the parse tree.</returns>
    public VBADateParser.DateLiteralContext Parse(string date)
    {
        var stream = new AntlrInputStream(date);
        var lexer = new VBADateLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new VBADateParser(tokens);

        VBADateParser.CompilationUnitContext tree;
        try
        {
            parser.AddErrorListener(new RubberduckParseErrorListener(null!, null!, PredictionMode.Sll));
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
            tree = parser.compilationUnit();
        }
        catch
        {
            tokens.Reset();
            parser.Reset();
            parser.Interpreter.PredictionMode = PredictionMode.Ll;
            parser.AddErrorListener(new RubberduckParseErrorListener(null!, null!, PredictionMode.Ll));
            tree = parser.compilationUnit();
        }
        return tree.dateLiteral();
    }
}

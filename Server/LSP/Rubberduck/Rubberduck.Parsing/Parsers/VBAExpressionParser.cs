using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Parsers
{
    public sealed class VBAExpressionParser
    {
        /// <summary>
        /// Parses the given VBA expression.
        /// </summary>
        /// <param name="expression">The expression to parse. NOTE: Call statements are not supported.</param>
        /// <returns>The root of the parse tree.</returns>
        public ParserRuleContext Parse(string expression)
        {
            var stream = new AntlrInputStream(expression);
            var lexer = new VBALexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new VBAParser(tokens);

            ParserRuleContext tree;
            try
            {
                parser.Interpreter.PredictionMode = PredictionMode.Sll;
                tree = parser.expression();
            }
            catch
            {
                tokens.Reset();
                parser.Reset();
                parser.Interpreter.PredictionMode = PredictionMode.Ll;
                tree = parser.expression();
            }
            return tree;
        }
    }
}
